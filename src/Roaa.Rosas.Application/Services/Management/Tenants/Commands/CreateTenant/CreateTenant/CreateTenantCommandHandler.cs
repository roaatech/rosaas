using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenant;

public partial class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantCreatedResultDto>>
{
    #region Props  
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    private Guid _orderId;
    #endregion

    #region Corts
    public CreateTenantCommandHandler(
        IRosasDbContext dbContext,
        ITenantWorkflow workflow,
        ILogger<CreateTenantCommandHandler> logger)
    {
        _dbContext = dbContext;
        _workflow = workflow;
        _logger = logger;
    }

    #endregion


    #region Handler   
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var initialProcess = await _workflow.GetNextStageAsync(expectedResourceStatus: ExpectedTenantResourceStatus.None,
                                                                currentStatus: TenantStatus.None,
                                                                currentStep: TenantStep.None,
                                                                userType: command.UserType);

        if (initialProcess is null)
        {
            throw new NullReferenceException($"The Initial Process of tenant workflow can't be null. [UserType:{command.UserType}]");
        }
        // first status
        var tenant = await CreateTenantInDBAsync(command, initialProcess, cancellationToken);

        var updatedStatuses = await _dbContext.Subscriptions
                                                .Where(x => x.TenantId == tenant.Id)
                                                .Select(x => new { x.Status, x.Step, x.ExpectedResourceStatus, x.ProductId })
                                                .ToListAsync(cancellationToken);

        List<ProductTenantCreatedResultDto> productTenantCreatedResults = new();
        foreach (var item in updatedStatuses)
        {
            var stages = await _workflow.GetNextStagesAsync(expectedResourceStatus: item.ExpectedResourceStatus,
                                                         currentStatus: item.Status,
                                                      currentStep: item.Step,
                                                      userType: command.UserType);
            productTenantCreatedResults.Add(new ProductTenantCreatedResultDto(
            item.ProductId,
            item.Status,
            stages.ToActionsResults()));
        }

        return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenant.Id, tenant.SystemName, productTenantCreatedResults));
    }

    #endregion


    #region Utilities   

    private async Task<Tenant> CreateTenantInDBAsync(CreateTenantCommand model, Workflow Workflow, CancellationToken cancellationToken = default)
    {
        var planIds = model.Subscriptions.Select(x => x.PlanId)
                                           .ToList();

        var tenant = BuildTenantEntity(model, Workflow);

        var statusHistory = BuildTenantStatusHistoryEntities(model, tenant.Id, tenant.Subscriptions, Workflow);

        var healthStatuses = BuildProductTenantHealthStatusEntities(tenant.Subscriptions);


        tenant.AddDomainEvent(new TenantProcessingCompletedEvent(
                                 TenantProcessType.RecordCreated,
                                 true,
                                 null,
                                 out _,
                                 tenant.Subscriptions.ToArray()));

        tenant.AddDomainEvent(new TenantCreatedInStoreEvent(tenant,
                                                            tenant.Subscriptions.First().ExpectedResourceStatus,
                                                            tenant.Subscriptions.First().Status,
                                                            tenant.Subscriptions.First().Step,
                                                            model.PlanDataList.First().Plan.TenancyType));


        _dbContext.Tenants.Add(tenant);

        _dbContext.TenantStatusHistory.AddRange(statusHistory);

        _dbContext.TenantHealthStatuses.AddRange(healthStatuses);



        var featuresIds = tenant.Subscriptions.SelectMany(x => x.SubscriptionFeatures.Select(x => x.FeatureId).ToList()).ToList();
        var plansPricesIds = tenant.Subscriptions.Select(x => x.PlanPriceId).ToList();


        var plans = await _dbContext.Plans.Where(x => planIds.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var plan in plans)
        {
            plan.IsSubscribed = true;
        }

        var features = await _dbContext.Features.Where(x => featuresIds.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var feature in features)
        {
            feature.IsSubscribed = true;
        }


        var planPrices = await _dbContext.PlanPrices.Where(x => plansPricesIds.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var planPrice in planPrices)
        {
            planPrice.IsSubscribed = true;
        }

        var res = await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }
    private Tenant BuildTenantEntity(CreateTenantCommand model, Workflow workflow)
    {
        model.PlanDataList?.ForEach(x =>
        {
            x.GeneratedSubscriptionCycleId = Guid.NewGuid();
            x.GeneratedSubscriptionId = Guid.NewGuid();

            x.Features?.ForEach(f =>
            {
                f.GeneratedSubscriptionFeatureId = Guid.NewGuid();
                f.GeneratedSubscriptionFeatureCycleId = Guid.NewGuid();
            });
        });

        var id = Guid.NewGuid();
        var name = model.SystemName.ToLower();
        var displayName = model.DisplayName;

        var res = new Tenant
        {
            Id = id,
            LastOrderId = model.OrderId,
            SystemName = name,
            DisplayName = displayName,
            CreatedByUserId = model.UserId,
            ModifiedByUserId = model.UserId,
            CreationDate = _date,
            ModificationDate = _date,
            Subscriptions = model.PlanDataList.Select(item => new Subscription
            {
                Id = item.GeneratedSubscriptionId,
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(item.PlanPrice.PlanCycle)
                                          .CalculateExpiryDate(_date,
                                                                item.PlanPrice.CustomPeriodInDays,
                                                                GetTrialPeriodInDays(item),
                                                                item.Plan.TenancyType),
                SubscriptionMode = GetSubscriptionMode(item),
                TenantId = id,
                PlanId = item.Plan.Id,
                PlanPriceId = item.PlanPrice.Id,
                ProductId = item.Product.Id,
                Status = workflow.NextStatus,
                Step = workflow.NextStep,
                IsActive = true,
                CreatedByUserId = model.UserId,
                ModifiedByUserId = model.UserId,
                CreationDate = _date,
                ModificationDate = _date,
                HealthCheckUrl = item.Product.Url,
                HealthCheckUrlIsOverridden = false,
                TrialPeriod = BuildSubscriptionTrialPeriodEntity(item),
                SubscriptionCycleId = item.GeneratedSubscriptionCycleId,
                SubscriptionCycles = new List<SubscriptionCycle>()
                {
                    new SubscriptionCycle()
                     {
                        Id = item.GeneratedSubscriptionCycleId,
                        StartDate = _date,
                        EndDate = PlanCycleManager.FromKey(item.PlanPrice.PlanCycle)
                                          .CalculateExpiryDate(_date,
                                                                item.PlanPrice.CustomPeriodInDays,
                                                                GetTrialPeriodInDays(item),
                                                                item.Plan.TenancyType),
                        TenantId = id,
                        PlanId = item.Plan.Id,
                        PlanPriceId = item.PlanPrice.Id,
                        ProductId = item.Product.Id,
                        Cycle = item.PlanPrice.PlanCycle,
                        PlanDisplayName = item.Plan.DisplayName,
                        CreatedByUserId = model.UserId,
                        ModifiedByUserId = model.UserId,
                        CreationDate = _date,
                        ModificationDate = _date,
                        Price = item.PlanPrice.Price,
                     }
                },
                SubscriptionFeatures = item.Features.Select(f =>
                {
                    var subscriptionFeature = new SubscriptionFeature
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionFeatureCycleId = f.GeneratedSubscriptionFeatureCycleId,
                        StartDate = FeatureResetManager.FromKey(f.FeatureReset).GetStartDate(_date),
                        EndDate = FeatureResetManager.FromKey(f.FeatureReset).GetExpiryDate(_date),
                        FeatureId = f.FeatureId,
                        PlanFeatureId = f.PlanFeatureId,
                        RemainingUsage = f.Limit,
                        CreatedByUserId = model.UserId,
                        ModifiedByUserId = model.UserId,
                        CreationDate = _date,
                        ModificationDate = _date,
                    };

                    _dbContext.SubscriptionFeatureCycles.AddRange(BuildSubscriptionFeatureCycleEntity(model, f, item));

                    return subscriptionFeature;
                }).ToList(),
                SpecificationsValues = item.Specifications.Select(x => new SpecificationValue
                {
                    Id = Guid.NewGuid(),
                    TenantId = id,
                    SpecificationId = x.SpecificationId,
                    Value = model.Subscriptions.Where(s => s.ProductId == item.Product.Id)
                                              .Select(s => s.Specifications)
                                              .SingleOrDefault()?
                                              .Where(s => s.SpecificationId == x.SpecificationId)
                                              .SingleOrDefault()?
                                              .Value,
                    CreatedByUserId = model.UserId,
                    ModifiedByUserId = model.UserId,
                    CreationDate = _date,
                    ModificationDate = _date,
                }).ToList(),

            }).ToList(),
            //  Orders = new List<Order> { BuildOrderEntity(id, name, displayName, model.PlanDataList) },
        };

        return res;
    }
    private SubscriptionFeatureCycle BuildSubscriptionFeatureCycleEntity(CreateTenantCommand model, PlanFeatureInfoModel planFeature, TenantCreationPreparationModel planInfo)
    {
        return new SubscriptionFeatureCycle()
        {
            Id = planFeature.GeneratedSubscriptionFeatureCycleId,
            SubscriptionFeatureId = planFeature.GeneratedSubscriptionFeatureId,
            StartDate = _date,
            EndDate = FeatureResetManager.FromKey(planFeature.FeatureReset).GetExpiryDate(_date),
            FeatureId = planFeature.FeatureId,
            Limit = planFeature.Limit,
            FeatureReset = planFeature.FeatureReset,
            FeatureType = planFeature.FeatureType,
            FeatureUnit = planFeature.FeatureUnit,
            TotalUsage = planFeature.Limit is null ? null : 0,
            RemainingUsage = planFeature.Limit,
            PlanCycle = planInfo.PlanPrice.PlanCycle,
            FeatureDisplayName = planFeature.FeatureDisplayName,
            PlanFeatureId = planFeature.PlanFeatureId,
            CreatedByUserId = model.UserId,
            ModifiedByUserId = model.UserId,
            CreationDate = _date,
            ModificationDate = _date,
            SubscriptionCycleId = planInfo.GeneratedSubscriptionCycleId,
            SubscriptionId = planInfo.GeneratedSubscriptionId,
        };
    }


    private SubscriptionTrialPeriod? BuildSubscriptionTrialPeriodEntity(TenantCreationPreparationModel model)
    {
        if (model.HasTrial)
        {
            int TrialPeriodInDays;
            Guid TrialPlanId;
            Guid TrialPlanPriceId;

            if (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan)
            {
                TrialPeriodInDays = model.Product.TrialPeriodInDays;
                TrialPlanId = model.Product.TrialPlanId.Value;
                TrialPlanPriceId = model.Product.TrialPlanPriceId.Value;
            }
            else
            {
                TrialPeriodInDays = model.Plan.TrialPeriodInDays;
                TrialPlanId = model.Plan.Id;
                TrialPlanPriceId = model.PlanPrice.Id;
            }


            return new SubscriptionTrialPeriod()
            {
                Id = Guid.NewGuid(),
                StartDate = _date,
                EndDate = _date.AddDays(TrialPeriodInDays),
                TrialPeriodInDays = TrialPeriodInDays,
                PlanId = TrialPlanId,
                PlanPriceId = TrialPlanPriceId,
            };
        }
        return null;
    }

    private int? GetTrialPeriodInDays(TenantCreationPreparationModel model)
    {
        int? trialPeriodInDays = null;

        if (model.HasTrial)
        {
            if (model.Product.TrialType == ProductTrialType.ProductHasTrialPlan)
            {
                trialPeriodInDays = model.Product.TrialPeriodInDays;
            }
            else
            {
                trialPeriodInDays = model.Plan.TrialPeriodInDays;
            }
        }
        return trialPeriodInDays;
    }
    private SubscriptionMode GetSubscriptionMode(TenantCreationPreparationModel model)
    {
        if (model.HasTrial)
        {
            return SubscriptionMode.Trial;
        }

        return SubscriptionMode.Regular;
    }

    private IEnumerable<TenantHealthStatus> BuildProductTenantHealthStatusEntities(ICollection<Subscription> subscriptions)
    {
        return subscriptions.Select(item => new TenantHealthStatus
        {
            Id = item.Id,
            SubscriptionId = item.Id,
            ProductId = item.ProductId,
            TenantId = item.TenantId,
            LastCheckDate = item.CreationDate,
            CheckDate = item.CreationDate,
            IsHealthy = false,
            IsChecked = false,
        });
    }

    private IEnumerable<TenantStatusHistory> BuildTenantStatusHistoryEntities(CreateTenantCommand model, Guid tenantId, ICollection<Subscription> subscriptions, Workflow initialProcess)
    {
        return subscriptions.Select(subscription => new TenantStatusHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = subscription.ProductId,
            SubscriptionId = subscription.Id,
            Status = initialProcess.NextStatus,
            Step = initialProcess.NextStep,
            PreviousStatus = initialProcess.CurrentStatus,
            PreviousStep = initialProcess.CurrentStep,
            OwnerId = model.UserId,
            OwnerType = model.UserType,
            CreationDate = _date,
            TimeStamp = _date,
            Message = initialProcess.Message
        });
    }


    #endregion
}

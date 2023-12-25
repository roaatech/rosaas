﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantInDB;

public partial class CreateTenantInDBCommandHandler : IRequestHandler<CreateTenantInDBCommand, Result<TenantCreatedResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IProductService _productService;
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<CreateTenantInDBCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    private Guid _orderId;
    #endregion

    #region Corts
    public CreateTenantInDBCommandHandler(
        IPublisher publisher,
        IRosasDbContext dbContext,
        ITenantWorkflow workflow,
        IProductService productService,
        IExternalSystemAPI externalSystemAPI,
        IIdentityContextService identityContextService,
        ILogger<CreateTenantInDBCommandHandler> logger)
    {
        _publisher = publisher;
        _dbContext = dbContext;
        _workflow = workflow;
        _productService = productService;
        _externalSystemAPI = externalSystemAPI;
        _identityContextService = identityContextService;
        _logger = logger;
    }

    #endregion


    #region Handler   
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantInDBCommand command, CancellationToken cancellationToken)
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

        return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenant.Id, _orderId, productTenantCreatedResults));
    }

    #endregion


    #region Utilities   

    private async Task<Tenant> CreateTenantInDBAsync(CreateTenantInDBCommand model, Workflow Workflow, CancellationToken cancellationToken = default)
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
        var productsIds = tenant.Subscriptions.Select(x => x.ProductId).ToList();


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

        var tenantSystemNames = await _dbContext.TenantSystemNames.Where(x => productsIds.Contains(x.ProductId) &&
                                                                  tenant.SystemName.ToUpper().Equals(x.TenantNormalizedSystemName))
                                                      .ToListAsync(cancellationToken);
        foreach (var tenantSystemName in tenantSystemNames)
        {
            tenantSystemName.TenantId = tenant.Id;
        }


        var res = await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }
    private Tenant BuildTenantEntity(CreateTenantInDBCommand model, Workflow workflow)
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
                EndDate = PlanCycleManager.FromKey(item.PlanPrice.PlanCycle).CalculateExpiryDate(_date, item.PlanPrice.CustomPeriodInDays),
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
                SubscriptionCycleId = item.GeneratedSubscriptionCycleId,
                SubscriptionCycles = new List<SubscriptionCycle>()
                {
                    new SubscriptionCycle()
                     {
                        Id = item.GeneratedSubscriptionCycleId,
                        StartDate = _date,
                        EndDate = PlanCycleManager.FromKey(item.PlanPrice.PlanCycle).CalculateExpiryDate(_date, item.PlanPrice.CustomPeriodInDays),
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
    private SubscriptionFeatureCycle BuildSubscriptionFeatureCycleEntity(CreateTenantInDBCommand model, PlanFeatureInfoModel planFeature, TenantCreationPreparationModel planInfo)
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

    private IEnumerable<TenantStatusHistory> BuildTenantStatusHistoryEntities(CreateTenantInDBCommand model, Guid tenantId, ICollection<Subscription> subscriptions, Workflow initialProcess)
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

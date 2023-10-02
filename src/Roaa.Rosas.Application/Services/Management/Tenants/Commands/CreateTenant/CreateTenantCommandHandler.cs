using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models.ExternalSystems;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public partial class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantCreatedResultDto>>
{
    #region Props 
    private readonly IPublisher _publisher;
    private readonly IRosasDbContext _dbContext;
    private readonly ITenantWorkflow _workflow;
    private readonly IProductService _productService;
    private readonly IExternalSystemAPI _externalSystemAPI;
    private readonly IIdentityContextService _identityContextService;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    private readonly DateTime _date = DateTime.UtcNow;
    #endregion

    #region Corts
    public CreateTenantCommandHandler(
        IPublisher publisher,
        IRosasDbContext dbContext,
        ITenantWorkflow workflow,
        IProductService productService,
        IExternalSystemAPI externalSystemAPI,
        IIdentityContextService identityContextService,
        ILogger<CreateTenantCommandHandler> logger)
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
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        #region Validation  
        var planPriceIds = request.Subscriptions.Select(x => x.PlanPriceId).ToList();

        var plansInfo = await _dbContext.PlanPrices
                                        .AsNoTracking()
                                        .Where(x => planPriceIds.Contains(x.Id))
                                        .Select(x => new PlanInfoModel
                                        {
                                            PlanName = x.Plan.Name,
                                            Price = x.Price,
                                            PlanPriceId = x.Id,
                                            PlanId = x.PlanId,
                                            IsPublished = x.Plan.IsPublished,
                                            PlanCycle = x.Cycle,
                                            Product = new ProductUrlListItem
                                            {
                                                Id = x.Plan.ProductId,
                                                Url = x.Plan.Product.DefaultHealthCheckUrl
                                            },

                                        })
                                        .ToListAsync(cancellationToken);

        if (plansInfo is null || !plansInfo.Any())
        {
            return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.ParameterIsRequired, _identityContextService.Locale, nameof(request.Subscriptions));
        }


        foreach (var item in plansInfo)
        {
            var req = request.Subscriptions.Where(x => x.ProductId == item.Product.Id).FirstOrDefault();
            if (req is null)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
            }

            if (request.Subscriptions.Where(x => x.ProductId == item.Product.Id).Count() > 1)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
            }

            if (!item.IsPublished)
            {
                return Result<TenantCreatedResultDto>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, nameof(request.Subscriptions));
            }
        }


        if (!await EnsureUniqueNameAsync(request.Subscriptions.Select(x => x.ProductId).ToList(), request.UniqueName))
        {
            return Result<TenantCreatedResultDto>.Fail(ErrorMessage.NameAlreadyUsed, _identityContextService.Locale, nameof(request.UniqueName));
        }

        #endregion

        // first status
        var tenant = await CreateTenantInDBAsync(request, plansInfo, cancellationToken);


        var updatedStatuses = await _dbContext.Subscriptions
                                         .Where(x => x.TenantId == tenant.Id)
                                         .Select(x => new { x.Status, x.ProductId })
                                         .ToListAsync(cancellationToken);

        List<ProductTenantCreatedResultDto> productTenantCreatedResults = new();
        foreach (var item in updatedStatuses)
        {
            productTenantCreatedResults.Add(new ProductTenantCreatedResultDto(
            item.ProductId,
            item.Status,
            (await _workflow.GetProcessActionsAsync(item.Status, _identityContextService.GetUserType()))
                            .ToActionsResults()));
        }



        return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenant.Id, productTenantCreatedResults));
    }

    #endregion


    #region Utilities   
    private async Task<Result<ExternalSystemResultModel<dynamic>>> CallExternalSystemToCreateTenantResourecesAsync(Tenant tenant, ProductUrlListItem item, CancellationToken cancellationToken)
    {
        return await _externalSystemAPI.CreateTenantAsync(new ExternalSystemRequestModel<CreateTenantModel>
        {
            BaseUrl = item.Url,
            TenantId = tenant.Id,
            Data = new()
            {
                TenantName = tenant.UniqueName,
            }
        }, cancellationToken);
    }


    private async Task<Tenant> CreateTenantInDBAsync(CreateTenantCommand request, List<PlanInfoModel> plansInfo, CancellationToken cancellationToken = default)
    {
        var initialProcess = await _workflow.GetNextProcessActionAsync(TenantStatus.None, _identityContextService.GetUserType());

        var planIds = request.Subscriptions.Select(x => x.PlanId).ToList();
        var featuresInfo = await _dbContext.PlanFeatures
                                            .AsNoTracking()
                                            .Where(x => planIds.Contains(x.PlanId))
                                            .Select(x => new FeatureInfoModel
                                            {
                                                PlanFeatureId = x.Id,
                                                FeatureId = x.FeatureId,
                                                Unit = x.Unit,
                                                PlanId = x.PlanId,
                                                Limit = x.Limit,
                                                Type = x.Feature.Type,
                                                Reset = x.Feature.Reset,
                                            })
                                            .ToListAsync(cancellationToken);

        foreach (var item in plansInfo)
        {
            item.Features = featuresInfo.Where(x => x.PlanId == item.PlanId).ToList();
        }

        var tenant = BuildTenantEntity(request, plansInfo, initialProcess);

        var statusHistory = BuildTenantStatusHistoryEntities(tenant.Id, tenant.Subscriptions, initialProcess);

        var processHistory = BuildTenantProcessHistoryEntities(tenant.Id, tenant.Subscriptions, initialProcess);

        var healthStatuses = BuildProductTenantHealthStatusEntities(tenant.Subscriptions);

        tenant.AddDomainEvent(new TenantCreatedInStoreEvent(tenant, tenant.Subscriptions.First().Status));

        _dbContext.Tenants.Add(tenant);

        _dbContext.TenantStatusHistory.AddRange(statusHistory);

        _dbContext.TenantProcessHistory.AddRange(processHistory);

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


        await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }
    private Tenant BuildTenantEntity(CreateTenantCommand model, List<PlanInfoModel> plansInfo, Process initialProcess)
    {
        plansInfo?.ForEach(x =>
        {
            // var subscriptionCycleId = Guid.NewGuid();
            x.GeneratedSubscriptionCycleId = Guid.NewGuid();
            x.GeneratedSubscriptionId = Guid.NewGuid();

            x.Features?.ForEach(f =>
            {
                f.GeneratedSubscriptionFeatureCycleId = Guid.NewGuid();
            });
        });

        var id = Guid.NewGuid();

        return new Tenant
        {
            Id = id,
            UniqueName = model.UniqueName.ToLower(),
            Title = model.Title,
            CreatedByUserId = _identityContextService.GetActorId(),
            ModifiedByUserId = _identityContextService.GetActorId(),
            CreationDate = _date,
            ModificationDate = _date,
            Subscriptions = plansInfo.Select(item => new Subscription
            {
                Id = item.GeneratedSubscriptionId,
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(item.PlanCycle).GetExpiryDate(_date),
                TenantId = id,
                PlanId = item.PlanId,
                PlanPriceId = item.PlanPriceId,
                ProductId = item.Product.Id,
                Status = initialProcess.NextStatus,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
                HealthCheckUrl = item.Product.Url,
                HealthCheckUrlIsOverridden = false,
                IsPaid = true,
                SubscriptionCycleId = item.GeneratedSubscriptionCycleId,
                SubscriptionCycles = new List<SubscriptionCycle>()
                {
                    new SubscriptionCycle()
                     {
                        Id = item.GeneratedSubscriptionCycleId,
                        StartDate = _date,
                        EndDate = PlanCycleManager.FromKey(item.PlanCycle).GetExpiryDate(_date),
                        TenantId = id,
                        PlanId = item.PlanId,
                        PlanPriceId = item.PlanPriceId,
                        ProductId = item.Product.Id,
                        Cycle = item.PlanCycle,
                        PlanName = item.PlanName,
                        CreatedByUserId = _identityContextService.GetActorId(),
                        ModifiedByUserId = _identityContextService.GetActorId(),
                        CreationDate = _date,
                        ModificationDate = _date,
                        Price = item.Price,
                     }
                },
                SubscriptionFeatures = item.Features.Select(f => new SubscriptionFeature
                {
                    Id = Guid.NewGuid(),
                    SubscriptionFeatureCycleId = f.GeneratedSubscriptionFeatureCycleId,
                    StartDate = _date,
                    EndDate = FeatureResetManager.FromKey(f.Reset).GetExpiryDate(_date),
                    FeatureId = f.FeatureId,
                    PlanFeatureId = f.PlanFeatureId,
                    RemainingUsage = f.Limit,
                    CreatedByUserId = _identityContextService.GetActorId(),
                    ModifiedByUserId = _identityContextService.GetActorId(),
                    CreationDate = _date,
                    ModificationDate = _date,
                    SubscriptionFeatureCycles = new List<SubscriptionFeatureCycle>()
                    {
                        new SubscriptionFeatureCycle()
                        {
                            Id = f.GeneratedSubscriptionFeatureCycleId,
                            StartDate = _date,
                            EndDate =  FeatureResetManager.FromKey(f.Reset).GetExpiryDate(_date),
                            FeatureId = f.FeatureId,
                            Limit = f.Limit,
                            Reset = f.Reset,
                            Type = f.Type,
                            Unit = f.Unit,
                            TotalUsage = f.Limit is null ? null : 0,
                            RemainingUsage = f.Limit,
                            Cycle = item.PlanCycle,
                            FeatureName = f.Name,
                            PlanFeatureId = f.PlanFeatureId,
                            CreatedByUserId = _identityContextService.GetActorId(),
                            ModifiedByUserId = _identityContextService.GetActorId(),
                            CreationDate = _date,
                            ModificationDate = _date,
                            SubscriptionCycleId = item.GeneratedSubscriptionCycleId,
                            SubscriptionId = item.GeneratedSubscriptionId,
                        }
                    },
                }).ToList()
            }).ToList(),
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
            LastCheckDate = item.ModificationDate,
            CheckDate = item.ModificationDate,
            IsHealthy = false,
        });
    }

    private IEnumerable<TenantStatusHistory> BuildTenantStatusHistoryEntities(Guid tenantId, ICollection<Subscription> subscriptions, Process initialProcess)
    {


        return subscriptions.Select(subscription => new TenantStatusHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = subscription.ProductId,
            SubscriptionId = subscription.Id,
            Status = initialProcess.NextStatus,
            PreviousStatus = initialProcess.CurrentStatus,
            OwnerId = _identityContextService.GetActorId(),
            OwnerType = _identityContextService.GetUserType(),
            Created = _date,
            TimeStamp = _date,
            Message = initialProcess.Message
        });
    }
    private IEnumerable<TenantProcessHistory> BuildTenantProcessHistoryEntities(Guid tenantId, ICollection<Subscription> subscriptions, Process initialProcess)
    {


        return subscriptions.Select(subscription => new TenantProcessHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProductId = subscription.ProductId,
            SubscriptionId = subscription.Id,
            Status = initialProcess.NextStatus,
            OwnerId = _identityContextService.GetActorId(),
            OwnerType = _identityContextService.GetUserType(),
            ProcessDate = _date,
            TimeStamp = _date,
            ProcessType = TenantProcessType.RecordCreated
        });
    }
    private async Task<bool> EnsureUniqueNameAsync(List<Guid> productsIds, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Subscriptions
                                .Where(x => x.TenantId != id && x.Tenant != null &&
                                            productsIds.Contains(x.ProductId) &&
                                            uniqueName.ToLower().Equals(x.Tenant.UniqueName))
                                .AnyAsync(cancellationToken);
    }


    #endregion
}

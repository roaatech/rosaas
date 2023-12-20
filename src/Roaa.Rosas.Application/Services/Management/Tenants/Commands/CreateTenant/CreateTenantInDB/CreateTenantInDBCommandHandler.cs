using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.IdentityContextUtilities;
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
    public async Task<Result<TenantCreatedResultDto>> Handle(CreateTenantInDBCommand request, CancellationToken cancellationToken)
    {

        // first status
        var tenant = await CreateTenantInDBAsync(request, cancellationToken);


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
                                                      userType: _identityContextService.GetUserType());
            productTenantCreatedResults.Add(new ProductTenantCreatedResultDto(
            item.ProductId,
            item.Status,
            stages.ToActionsResults()));
        }

        return Result<TenantCreatedResultDto>.Successful(new TenantCreatedResultDto(tenant.Id, _orderId, productTenantCreatedResults));
    }

    #endregion


    #region Utilities   

    private async Task<Tenant> CreateTenantInDBAsync(CreateTenantInDBCommand request, CancellationToken cancellationToken = default)
    {
        var planIds = request.Subscriptions.Select(x => x.PlanId)
                                           .ToList();

        var tenant = BuildTenantEntity(request);

        var statusHistory = BuildTenantStatusHistoryEntities(tenant.Id, tenant.Subscriptions, request.Workflow);

        var healthStatuses = BuildProductTenantHealthStatusEntities(tenant.Subscriptions);


        tenant.AddDomainEvent(new TenantProcessingCompletedEvent(
                                 TenantProcessType.RecordCreated,
                                 true,
                                 null,
                                 out _,
                                 tenant.Subscriptions.ToArray()));

        tenant.AddDomainEvent(new TenantCreatedInStoreEvent(tenant, tenant.Subscriptions.First().ExpectedResourceStatus, tenant.Subscriptions.First().Status, tenant.Subscriptions.First().Step));


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
    private Tenant BuildTenantEntity(CreateTenantInDBCommand model)
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
        var name = model.UniqueName.ToLower();
        var displayName = model.Title;
        var dfdddf = PlanCycleManager.FromKey(model.PlanDataList[0].PlanCycle).CalculateExpiryDate(_date, model.PlanDataList[0].CustomPeriodInDays);
        var res = new Tenant
        {
            Id = id,
            SystemName = name,
            DisplayName = displayName,
            CreatedByUserId = _identityContextService.GetActorId(),
            ModifiedByUserId = _identityContextService.GetActorId(),
            CreationDate = _date,
            ModificationDate = _date,
            Subscriptions = model.PlanDataList.Select(item => new Subscription
            {
                Id = item.GeneratedSubscriptionId,
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(item.PlanCycle).CalculateExpiryDate(_date, item.CustomPeriodInDays),
                TenantId = id,
                PlanId = item.PlanId,
                PlanPriceId = item.PlanPriceId,
                ProductId = item.Product.Id,
                Status = model.Workflow.NextStatus,
                Step = model.Workflow.NextStep,
                IsActive = true,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
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
                        EndDate = PlanCycleManager.FromKey(item.PlanCycle).CalculateExpiryDate(_date, item.CustomPeriodInDays),
                        TenantId = id,
                        PlanId = item.PlanId,
                        PlanPriceId = item.PlanPriceId,
                        ProductId = item.Product.Id,
                        Cycle = item.PlanCycle,
                        PlanDisplayName = item.PlanDisplayName,
                        CreatedByUserId = _identityContextService.GetActorId(),
                        ModifiedByUserId = _identityContextService.GetActorId(),
                        CreationDate = _date,
                        ModificationDate = _date,
                        Price = item.Price,
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
                        CreatedByUserId = _identityContextService.GetActorId(),
                        ModifiedByUserId = _identityContextService.GetActorId(),
                        CreationDate = _date,
                        ModificationDate = _date,
                    };

                    _dbContext.SubscriptionFeatureCycles.AddRange(BuildSubscriptionFeatureCycleEntity(f, item));

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
                    CreatedByUserId = _identityContextService.GetActorId(),
                    ModifiedByUserId = _identityContextService.GetActorId(),
                    CreationDate = _date,
                    ModificationDate = _date,
                }).ToList(),

            }).ToList(),
            Orders = new List<Order> { BuildOrderEntity(id, name, displayName, model.PlanDataList) },
        };

        return res;
    }

    private Order BuildOrderEntity(Guid tenantId, string tenantName, string tenantDisplayName, List<PlanDataModel> plansInfo)
    {
        var quantity = 1;

        var orderItems = plansInfo.Select(planInfo => new OrderItem()
        {
            Id = Guid.NewGuid(),
            StartDate = _date,
            EndDate = PlanCycleManager.FromKey(planInfo.PlanCycle).CalculateExpiryDate(_date, planInfo.CustomPeriodInDays),
            ClientId = planInfo.Product.ClientId,
            ProductId = planInfo.Product.Id,
            SubscriptionId = planInfo.GeneratedSubscriptionId,
            PurchasedEntityId = planInfo.PlanId,
            PurchasedEntityType = Common.Enums.EntityType.Plan,
            PriceExclTax = planInfo.Price * quantity,
            PriceInclTax = planInfo.Price * quantity,
            UnitPriceExclTax = planInfo.Price,
            UnitPriceInclTax = planInfo.Price,
            Quantity = quantity,
            SystemName = $"{planInfo.Product.Name}--{planInfo.PlanName}--{tenantName}",
            DisplayName = $"[Product: {planInfo.Product.DisplayName}], [Plan: {planInfo.PlanDisplayName}], [Tenant: {tenantDisplayName}]",
            Specifications = planInfo.Features.Select(x => new OrderItemSpecification
            {
                PurchasedEntityId = x.FeatureId,
                PurchasedEntityType = Common.Enums.EntityType.Feature,
                Name = $"{x.FeatureName}-" +
                                $"{(x.Limit.HasValue ? x.Limit : string.Empty)}-" +
                                $"{(x.FeatureUnit.HasValue ? x.FeatureUnit.ToString() : string.Empty)}-" +
                                $"{(x.FeatureReset != FeatureReset.NonResettable ? x.FeatureReset.ToString() : string.Empty)}"
                                .Replace("---", "-")
                                .Replace("--", "-")
                                .TrimEnd('-'),
            }).ToList()
        }).ToList();

        _orderId = Guid.NewGuid();

        return new Order()
        {
            Id = _orderId,
            TenantId = tenantId,
            OrderStatus = OrderStatus.Pending,
            CurrencyRate = 1,
            UserCurrencyType = CurrencyCode.USD,
            UserCurrencyCode = CurrencyCode.USD.ToString(),
            PaymentStatus = null,
            PaymentMethodType = null,
            CreatedByUserType = _identityContextService.GetUserType(),
            CreatedByUserId = _identityContextService.GetActorId(),
            ModifiedByUserId = _identityContextService.GetActorId(),
            CreationDate = _date,
            ModificationDate = _date,
            OrderSubtotalExclTax = orderItems.Select(x => x.PriceExclTax).Sum(),
            OrderSubtotalInclTax = orderItems.Select(x => x.PriceInclTax).Sum(),
            OrderTotal = orderItems.Select(x => x.PriceInclTax).Sum(),
            OrderItems = orderItems,
        };
    }

    private SubscriptionFeatureCycle BuildSubscriptionFeatureCycleEntity(PlanFeatureInfoModel planFeature, PlanDataModel planInfo)
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
            PlanCycle = planInfo.PlanCycle,
            FeatureDisplayName = planFeature.FeatureDisplayName,
            PlanFeatureId = planFeature.PlanFeatureId,
            CreatedByUserId = _identityContextService.GetActorId(),
            ModifiedByUserId = _identityContextService.GetActorId(),
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

    private IEnumerable<TenantStatusHistory> BuildTenantStatusHistoryEntities(Guid tenantId, ICollection<Subscription> subscriptions, Workflow initialProcess)
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
            OwnerId = _identityContextService.GetActorId(),
            OwnerType = _identityContextService.GetUserType(),
            CreationDate = _date,
            TimeStamp = _date,
            Message = initialProcess.Message
        });
    }

    private async Task<bool> EnsureUniqueNameAsync(List<Guid> productsIds, string uniqueName, Guid id = new Guid(), CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Subscriptions
                                .Where(x => x.TenantId != id && x.Tenant != null &&
                                            productsIds.Contains(x.ProductId) &&
                                            uniqueName.ToLower().Equals(x.Tenant.SystemName))
                                .AnyAsync(cancellationToken);
    }


    #endregion
}

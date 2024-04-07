using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Utilities;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public partial class SubscriptionService : ISubscriptionService
    {
        #region Props 
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly IMediator _mediator;
        private DateTime _date;
        #endregion


        #region Corts
        public SubscriptionService(ILogger<SubscriptionService> logger,
                                   IIdentityContextService identityContextService,
                                   IGenericAttributeService genericAttributeService,
                                   IRosasDbContext dbContext,
                                   IPublisher publisher,
                                   IMediator mediator)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _genericAttributeService = genericAttributeService;
            _dbContext = dbContext;
            _publisher = publisher;
            _mediator = mediator;
            _date = DateTime.UtcNow;
        }

        #endregion


        #region Services  
        public async Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                        .Include(x => x.Feature)
                                                        .Where(x => x.Subscription.StartDate <= date &&
                                                                    x.Subscription.EndDate > date &&
                                                                    x.Subscription.IsActive &&
                                                                    x.RemainingUsage != null &&
                                                                    x.StartDate <= date &&
                                                                    x.EndDate != null &&
                                                                    x.EndDate < date)
                                                       .ToListAsync();
            return await ResetSubscriptionsFeaturesAsync(subscriptionFeatures: subscriptionFeatures,
                                                         comment: null,
                                                         systemComment: "Reset by the System",
                                                         cancellationToken: cancellationToken);
        }

        public async Task<Result> ResetSubscriptionsFeaturesAsync(List<SubscriptionFeature> subscriptionFeatures, string? comment, string? systemComment, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;

            if (subscriptionFeatures.Any())
            {


                var featureCyclesIds = subscriptionFeatures
                                                .Select(x => x.SubscriptionFeatureCycleId)
                                                .ToList();

                var featureCycles = await _dbContext.SubscriptionFeatureCycles
                                                           .Where(x => featureCyclesIds.Contains(x.Id))
                                                           .ToListAsync();



                if (featureCycles.Select(x => x.FeatureReset).Where(reset => FeatureResetManager.FromKey(reset).IsResettable()).Any())
                {

                    var subscriptionsIds = new List<Guid>();

                    // Adds a new cycle to the subscription's features
                    foreach (var subscriptionFeature in subscriptionFeatures)
                    {
                        var cycle = featureCycles
                                        .Where(x => x.Id == subscriptionFeature.SubscriptionFeatureCycleId)
                                        .FirstOrDefault();

                        if (FeatureResetManager.FromKey(cycle.FeatureReset).IsResettable())
                        {
                            var featureResetManager = FeatureResetManager.FromKey(cycle.FeatureReset);

                            // Adds a new cycle to the current subscription's feature 
                            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                            {
                                Id = Guid.NewGuid(),
                                StartDate = featureResetManager.GetStartDate(_date),
                                EndDate = featureResetManager.GetExpiryDate(date),
                                SubscriptionId = subscriptionFeature.SubscriptionId,
                                SubscriptionCycleId = cycle.SubscriptionCycleId,
                                SubscriptionFeatureId = subscriptionFeature.Id,
                                FeatureId = cycle.FeatureId,
                                PlanFeatureId = cycle.PlanFeatureId,
                                Limit = cycle.Limit,
                                FeatureReset = cycle.FeatureReset,
                                FeatureType = cycle.FeatureType,
                                FeatureUnit = cycle.FeatureUnit,
                                TotalUsage = cycle.Limit is null ? null : 0,
                                RemainingUsage = cycle.Limit,
                                PlanCycle = cycle.PlanCycle,
                                FeatureDisplayName = cycle.FeatureDisplayName,
                                CreatedByUserId = _identityContextService.GetActorId(),
                                ModifiedByUserId = _identityContextService.GetActorId(),
                                CreationDate = date,
                                ModificationDate = date,
                            };


                            // update the subscription's feature by the new cycle of current subscription's feature 
                            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
                            subscriptionFeature.StartDate = subscriptionFeatureCycle.StartDate;
                            subscriptionFeature.EndDate = subscriptionFeatureCycle.EndDate;
                            subscriptionFeature.RemainingUsage = subscriptionFeatureCycle.Limit;

                            _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);


                            subscriptionsIds.Add(subscriptionFeature.SubscriptionId);
                        }

                    }
                    subscriptionsIds = subscriptionsIds.Distinct().ToList();


                    // retrieving the subscriptions 
                    var subscriptions = await _dbContext.Subscriptions
                                                        .Where(x => subscriptionsIds.Contains(x.Id))
                                                        .ToListAsync();

                    // update the LastLimitsResetDate property value of subscription's 
                    foreach (var subscription in subscriptions)
                    {


                        subscription.LastLimitsResetDate = date;

                        var subscriptionFeatureItems = subscriptionFeatures
                                                            .Where(x => x.SubscriptionId == subscription.Id)
                                                            .Select(x => new SubscriptionFeatureItemModel(
                                                                                            subscriptionFeatureId: x.Id,
                                                                                            name: x.Feature?.SystemName ?? ""))
                                                            .ToList();

                        subscription.AddDomainEvent(new SubscriptionFeaturesLimitsResetEvent(
                                                                     subscriptionFeatures: subscriptionFeatureItems,
                                                                     subscription: subscription,
                                                                     comment: comment,
                                                                     systemComment: systemComment));

                    }



                    await _dbContext.SaveChangesAsync();
                }
            }

            return Result.Successful();
        }

        public async Task<List<Subscription>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= _date &&
                                                            x.EndDate < _date &&
                                                            x.IsActive
                                                          )
                                                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Changing subscription plan that needs to change its plan 
        /// </summary> 




        public async Task<Result> ResetSubscriptionPlanAsync(Subscription subscription,
                                                                Guid planId,
                                                                Guid planPriceId,
                                                                CancellationToken cancellationToken = default)
        {
            var planPrice = await _dbContext.PlanPrices
                                         .AsNoTracking()
                                         .Include(x => x.Plan)
                                         .Where(x => x.Id == planPriceId)
                                         .SingleOrDefaultAsync(cancellationToken);
            ArgumentNullException.ThrowIfNull(planPrice);

            await RenewSubscriptionAsync(subscription,
                                            planId,
                                            planPriceId,
                                            planPrice.PlanCycle,
                                            planPrice.Price,
                                            planPrice.Plan.DisplayName,
                                            keepCurrentSubscriptionFeatures: false,
                                            cancellationToken);

            return Result.Successful();
        }


        private List<PlanFeatureInfoModel> _subscriptionPlanFeatures;
        public async Task<List<PlanFeatureInfoModel>> FetchSubscriptionPlanFeaturesAsync(Subscription subscription,
                                                                                                SubscriptionRenewal subscriptionRenewal,
                                                                                                CancellationToken cancellationToken = default)
        {
            if (_subscriptionPlanFeatures is not null) return _subscriptionPlanFeatures;

            _subscriptionPlanFeatures = await _dbContext.PlanFeatures
                                                   .AsNoTracking()
                                                   .Where(x => x.PlanId == subscriptionRenewal.PlanId)
                                                   .Select(x => new PlanFeatureInfoModel
                                                   {
                                                       PlanFeatureId = x.Id,
                                                       FeatureId = x.FeatureId,
                                                       FeatureUnit = x.FeatureUnit,
                                                       PlanId = x.PlanId,
                                                       Limit = x.Limit,
                                                       FeatureDisplayName = x.Feature.DisplayName,
                                                       FeatureType = x.Feature.Type,
                                                       FeatureReset = x.FeatureReset,
                                                   })
                                                   .ToListAsync(cancellationToken);
            return _subscriptionPlanFeatures;
        }

        public async Task RenewSubscriptionAsync(Subscription subscription, SubscriptionRenewal subscriptionRenewal, bool keepCurrentSubscriptionFeatures, CancellationToken cancellationToken = default)
        {
            var subscriptionRenewalHistory = new SubscriptionRenewalHistory
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscriptionRenewal.SubscriptionId,
                PlanId = subscriptionRenewal.PlanId,
                PlanPriceId = subscriptionRenewal.PlanPriceId,
                Type = subscriptionRenewal.Type,
                PlanCycle = subscriptionRenewal.PlanCycle,
                Price = subscriptionRenewal.Price,
                RenewalEnabledByUserId = subscriptionRenewal.ModifiedByUserId,
                RenewalEnabledDate = subscriptionRenewal.ModificationDate,
                Comment = subscriptionRenewal.Comment,
                RenewalDate = DateTime.UtcNow,
            };

            _dbContext.SubscriptionRenewalHistories.Add(subscriptionRenewalHistory);

            await RenewSubscriptionAsync(subscription,
                                    subscriptionRenewal.PlanId,
                                    subscriptionRenewal.PlanPriceId,
                                    subscriptionRenewal.PlanCycle,
                                    subscriptionRenewal.Price,
                                    subscriptionRenewal.PlanDisplayName,
                                    keepCurrentSubscriptionFeatures: keepCurrentSubscriptionFeatures,
                                    cancellationToken);

        }

        private async Task RenewSubscriptionAsync(Subscription subscription,
                                              Guid planId,
                                              Guid planPriceId,
                                              PlanCycle planCycle,
                                              decimal price,
                                              string planDisplayName,
                                              bool keepCurrentSubscriptionFeatures,
                                              CancellationToken cancellationToken = default)
        {
            // #1
            var subscriptionCycle = new SubscriptionCycle()
            {
                Id = Guid.NewGuid(),
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(planCycle).CalculateExpiryDate(_date, null),
                SubscriptionId = subscription.Id,
                TenantId = subscription.TenantId,
                PlanId = planId,
                PlanPriceId = planPriceId,
                ProductId = subscription.ProductId,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
                PlanDisplayName = planDisplayName,
                Cycle = planCycle,
                Price = price,
                Type = SubscriptionCycleType.Normal,
            };
            _dbContext.SubscriptionCycles.Add(subscriptionCycle);


            // #2   Update Subscription Entity
            subscription.SubscriptionCycleId = subscriptionCycle.Id;
            subscription.StartDate = subscriptionCycle.StartDate;
            subscription.EndDate = subscriptionCycle.EndDate;
            subscription.PlanId = planId;
            subscription.PlanPriceId = planPriceId;
            subscription.ModificationDate = DateTime.UtcNow;



            var currentSubscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                        .Where(x => x.SubscriptionId == subscription.Id)
                                                        .ToListAsync();



            var planFeaturesInfo = await _dbContext.PlanFeatures.AsNoTracking()
                                            .Where(x => x.PlanId == planId)
                                            .Select(x => new
                                            {
                                                SubscriptionFeature = new SubscriptionFeature
                                                {
                                                    Id = Guid.NewGuid(),
                                                    PlanFeatureId = x.Id,
                                                    SubscriptionId = subscription.Id,
                                                    StartDate = _date,
                                                    EndDate = FeatureResetManager.FromKey(x.FeatureReset).GetExpiryDate(_date),
                                                    FeatureId = x.FeatureId,
                                                    RemainingUsage = x.Limit,
                                                    CreatedByUserId = _identityContextService.GetActorId(),
                                                    ModifiedByUserId = _identityContextService.GetActorId(),
                                                    CreationDate = _date,
                                                    ModificationDate = _date,

                                                },
                                                FeatureDisplayName = x.Feature.DisplayName,
                                                FeatureType = x.Feature.Type,
                                                x.FeatureReset,
                                                x.Limit,
                                                x.FeatureUnit
                                            }).ToListAsync(cancellationToken);
            if (!keepCurrentSubscriptionFeatures)
            {
                _dbContext.SubscriptionFeatures.RemoveRange(currentSubscriptionFeatures);
                _dbContext.SubscriptionFeatures.AddRange(planFeaturesInfo.Select(x => x.SubscriptionFeature));
            }


            foreach (var planFeatureInfo in planFeaturesInfo)
            {
                var generatedSubscriptionFeatureCycleId = Guid.NewGuid();
                SubscriptionFeature subscriptionFeature = planFeatureInfo.SubscriptionFeature;


                if (keepCurrentSubscriptionFeatures)
                {
                    var sf = currentSubscriptionFeatures.Where(x => x.FeatureId == planFeatureInfo.SubscriptionFeature.FeatureId)
                                                                      .SingleOrDefault();
                    ArgumentNullException.ThrowIfNull(sf);
                    subscriptionFeature = sf;

                    subscriptionFeature.StartDate = planFeatureInfo.SubscriptionFeature.StartDate;
                    subscriptionFeature.EndDate = planFeatureInfo.SubscriptionFeature.EndDate;
                    subscriptionFeature.RemainingUsage = planFeatureInfo.Limit;
                    subscriptionFeature.SubscriptionFeatureCycleId = generatedSubscriptionFeatureCycleId;
                }

                // #3
                var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                {
                    Id = generatedSubscriptionFeatureCycleId,
                    StartDate = subscriptionFeature.StartDate,
                    EndDate = subscriptionFeature.EndDate,
                    SubscriptionId = subscriptionFeature.SubscriptionId,
                    SubscriptionCycleId = subscriptionCycle.Id,
                    SubscriptionFeatureId = subscriptionFeature.Id,
                    FeatureId = subscriptionFeature.FeatureId,
                    PlanFeatureId = subscriptionFeature.PlanFeatureId,
                    Limit = planFeatureInfo.Limit,
                    FeatureReset = planFeatureInfo.FeatureReset,
                    FeatureType = planFeatureInfo.FeatureType,
                    FeatureUnit = planFeatureInfo.FeatureUnit,
                    TotalUsage = planFeatureInfo.Limit is null ? null : 0,
                    RemainingUsage = planFeatureInfo.Limit,
                    PlanCycle = planCycle,
                    FeatureDisplayName = planFeatureInfo.FeatureDisplayName,
                    CreatedByUserId = _identityContextService.GetActorId(),
                    ModifiedByUserId = _identityContextService.GetActorId(),
                    CreationDate = _date,
                    ModificationDate = _date,
                };
                _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);

                // #4  UpdateSubscriptionFeatureEntity
                planFeatureInfo.SubscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }



        public async Task<Result> SuspendSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
        {

            subscription.IsActive = false;
            subscription.ModificationDate = _date;
            subscription.Comment = "The subscription for the tenant has been suspended due to non-renewal.";
            subscription.AddDomainEvent(new SubscriptionWasSetAsInactiveDueToUnpaideEvent(subscription, subscription.Comment));
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _genericAttributeService.SaveAttributeAsync<SubscriptionRenewal, ExpectedTenantResourceStatus?>(
                                                                             subscription.Id,
                                                                             Consts.GenericAttributeKey.LastExpectedTenantResourceStatus,
                                                                             subscription.ExpectedResourceStatus,
                                                                             cancellationToken);

            if (subscription.ExpectedResourceStatus == ExpectedTenantResourceStatus.Active)
            {
                await _mediator.Send(new ChangeTenantStatusByIdCommand(subscription.TenantId,
                                                                       TenantStatus.SendingDeactivationRequest,
                                                                       subscription.ProductId,
                                                                       subscription.Comment),
                                                                       cancellationToken);
            }

            return Result.Successful();
        }

        public async Task<Result> ActivateSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default)
        {
            var lastExpectedTenantResourceStatus = await _genericAttributeService.GetAttributeAsync<Subscription, ExpectedTenantResourceStatus?>(
                                                                                                    subscription.Id,
                                                                                                    Consts.GenericAttributeKey.LastExpectedTenantResourceStatus,
                                                                                                    null,
                                                                                                    cancellationToken);
            if (!subscription.IsActive)
            {
                subscription.IsActive = true;
                subscription.ModificationDate = _date;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            if (lastExpectedTenantResourceStatus is null && subscription.ExpectedResourceStatus == ExpectedTenantResourceStatus.Inactive)
            {
                await ChangeTenantStatusAsync();
            }
            else if (lastExpectedTenantResourceStatus is not null &&
                     lastExpectedTenantResourceStatus != subscription.ExpectedResourceStatus &&
                     lastExpectedTenantResourceStatus == ExpectedTenantResourceStatus.Active)
            {
                await ChangeTenantStatusAsync();
            }

            if (lastExpectedTenantResourceStatus is not null)
            {
                await _genericAttributeService.DeleteAttributeAsync(subscription, Consts.GenericAttributeKey.LastExpectedTenantResourceStatus, cancellationToken);
            }

            async Task ChangeTenantStatusAsync()
            {
                await _mediator.Send(new ChangeTenantStatusByIdCommand(subscription.TenantId,
                                                                      TenantStatus.SendingActivationRequest,
                                                                      subscription.ProductId,
                                                                      subscription.Comment),
                                                                      cancellationToken);
            }

            return Result.Successful();
        }


        public async Task<Result> Temp__RenewSubscriptionsAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptions = await _dbContext.Subscriptions
                                                .Include(x => x.Plan)
                                                .Include(x => x.PlanPrice)
                                                .Include(x => x.SubscriptionFeatures)
                                                .Where(x => x.Id == subscriptionId)
                                                .ToListAsync();

            if (subscriptions.Any())
            {
                var cyclesIds = subscriptions.Select(x => x.SubscriptionCycleId).ToList();

                var cycles = await _dbContext.SubscriptionCycles
                                                           .Where(x => cyclesIds.Contains(x.Id))
                                                           .ToListAsync();

                foreach (var subscription in subscriptions)
                {
                    var cycle = cycles.Where(x => x.Id == subscription.SubscriptionCycleId).FirstOrDefault();

                    var subscriptionCycle = new SubscriptionCycle()
                    {
                        Id = Guid.NewGuid(),
                        StartDate = date,
                        EndDate = PlanCycleManager.FromKey(subscription.PlanPrice.PlanCycle).CalculateExpiryDate(date, null),
                        TenantId = subscription.TenantId,
                        PlanId = subscription.PlanId,
                        PlanPriceId = subscription.PlanPriceId,
                        ProductId = subscription.ProductId,
                        Cycle = subscription.PlanPrice.PlanCycle,
                        PlanDisplayName = subscription.Plan.DisplayName,
                        CreatedByUserId = subscription.CreatedByUserId,
                        ModifiedByUserId = subscription.ModifiedByUserId,
                        CreationDate = date,
                        ModificationDate = date,
                        Price = subscription.PlanPrice.Price,
                        SubscriptionId = subscription.Id,
                        Type = SubscriptionCycleType.Normal,
                    };


                    subscription.SubscriptionCycleId = subscriptionCycle.Id;
                    subscription.StartDate = subscriptionCycle.StartDate;
                    subscription.EndDate = subscriptionCycle.EndDate;


                    if (subscription.SubscriptionFeatures.Any())
                    {
                        var featureCyclesIds = subscription.SubscriptionFeatures.Select(x => x.SubscriptionFeatureCycleId).ToList();

                        var featureCycles = await _dbContext.SubscriptionFeatureCycles
                                                                   .Where(x => featureCyclesIds.Contains(x.Id))
                                                                   .ToListAsync();

                        foreach (var subscriptionFeature in subscription.SubscriptionFeatures)
                        {
                            var featureCycle = featureCycles.Where(x => x.Id == subscriptionFeature.SubscriptionFeatureCycleId).FirstOrDefault();

                            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                            {
                                Id = Guid.NewGuid(),
                                StartDate = date,
                                EndDate = FeatureResetManager.FromKey(featureCycle.FeatureReset).GetExpiryDate(date),
                                SubscriptionId = featureCycle.SubscriptionId,
                                SubscriptionCycleId = subscriptionCycle.Id,
                                SubscriptionFeatureId = subscriptionFeature.Id,
                                FeatureId = featureCycle.FeatureId,
                                PlanFeatureId = featureCycle.PlanFeatureId,
                                Limit = featureCycle.Limit,
                                FeatureReset = featureCycle.FeatureReset,
                                FeatureType = featureCycle.FeatureType,
                                FeatureUnit = featureCycle.FeatureUnit,
                                TotalUsage = featureCycle.Limit is null ? null : 0,
                                RemainingUsage = featureCycle.Limit,
                                PlanCycle = featureCycle.PlanCycle,
                                FeatureDisplayName = featureCycle.FeatureDisplayName,
                                CreatedByUserId = featureCycle.CreatedByUserId,
                                ModifiedByUserId = featureCycle.ModifiedByUserId,
                                CreationDate = date,
                                ModificationDate = date,
                            };

                            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
                            subscriptionFeature.StartDate = subscriptionFeatureCycle.StartDate;
                            subscriptionFeature.EndDate = subscriptionFeatureCycle.EndDate;
                            subscriptionFeature.RemainingUsage = subscriptionFeatureCycle.Limit;

                            _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);
                        }

                    }



                    _dbContext.SubscriptionCycles.Add(subscriptionCycle);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Result.Successful();
        }

        public async Task<Result> Temp__EndSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscription = await _dbContext.Subscriptions
                                                .Where(x => x.Id == subscriptionId)
                                                .SingleOrDefaultAsync();

            subscription.StartDate = DateTime.UtcNow.AddDays(-2);
            subscription.EndDate = DateTime.UtcNow.AddDays(-1);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Successful();
        }
        #endregion

    }
}

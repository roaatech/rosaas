using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Props 
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        private DateTime _date;
        #endregion


        #region Corts
        public SubscriptionService(ILogger<SubscriptionService> logger,
                                   IIdentityContextService identityContextService,
                                   IRosasDbContext dbContext,
                                   IPublisher publisher)
        {
            _logger = logger;
            _identityContextService = identityContextService;
            _dbContext = dbContext;
            _publisher = publisher;
            _date = DateTime.UtcNow;
        }

        #endregion


        #region Services 
        public async Task<Result> DeactivateSubscriptionDueToNonPaymentAsync(int periodTimeAfterEndDateInHours, CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var toDate = DateTime.UtcNow.AddHours(periodTimeAfterEndDateInHours);
            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= date &&
                                                            x.EndDate < toDate &&
                                                            (x.Status == TenantStatus.Active ||
                                                             x.Status == TenantStatus.CreatedAsActive))
                                                .ToListAsync();


            foreach (var subscription in subscriptions)
            {



                //await _publisher.Publish(new ExpirationOfAllowedActivationPeriodForUnpaidSubscriptionsEventHandler(subscription.TenantId,
                //                                                       TenantStatus.SendingDeactivationRequest,
                //                                                       subscription.ProductId,
                //                                                       "Deactivating the tenant due to non-payment of the subscription."),
                //                         cancellationToken);


                //await _mediator.Send(new ChangeTenantStatusByIdCommand(subscription.TenantId,
                //                                                       TenantStatus.SendingDeactivationRequest,
                //                                                       subscription.ProductId,
                //                                                       "Deactivating the tenant due to non-payment of the subscription."),
                //                         cancellationToken);

            }
            return Result.Successful();
        }

        public async Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                        .Include(x => x.Feature)
                                                        .Where(x => x.Subscription.StartDate <= date &&
                                                                   x.Subscription.EndDate > date &&
                                                                  x.Subscription.IsPaid &&
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
                                                                                            name: x.Feature?.Name ?? ""))
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

        public async Task<Result> RenewOrSetExpiredSubscriptionsAsUnpaidAsync(CancellationToken cancellationToken = default)
        {
            _date = DateTime.UtcNow;
            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= _date &&
                                                            x.EndDate < _date &&
                                                            x.IsPaid &&
                                                           (x.Status == TenantStatus.Active ||
                                                            x.Status == TenantStatus.CreatedAsActive))
                                                .ToListAsync();
            if (subscriptions.Any())
            {
                var subscriptionsIds = subscriptions.Select(x => x.Id).ToList();

                var subscriptionsFeatures = await _dbContext.SubscriptionFeatures
                                                              .Where(x => subscriptionsIds.Contains(x.SubscriptionId))
                                                              .ToListAsync();
                #region Subscriptions Renewal

                // Renew subscriptions that have enabled auto-renewal.

                var subscriptionAutoRenewals = await _dbContext.SubscriptionAutoRenewals
                                                               .Where(x => subscriptionsIds.Contains(x.SubscriptionId))
                                                               .ToListAsync();

                var subscriptionsIdsForAutoRenewal = subscriptionAutoRenewals.Select(x => x.SubscriptionId).ToList();

                var subscriptionsForRenewal = subscriptions.Where(x => subscriptionsIdsForAutoRenewal.Contains(x.Id));

                foreach (var subscription in subscriptionsForRenewal)
                {
                    var autoRenewal = subscriptionAutoRenewals.Where(x => x.SubscriptionId == subscription.Id).SingleOrDefault();

                    List<SubscriptionFeature> subscriptionFeatures = subscriptionsFeatures
                                                                        .Where(x => x.SubscriptionId == subscription.Id)
                                                                        .ToList();
                    // Renew subscription with different plan.
                    if (subscription.PlanId != autoRenewal.PlanId)
                    {
                        _dbContext.SubscriptionFeatures.RemoveRange(subscriptionFeatures);

                        var PlanFeatures = await _dbContext.PlanFeatures
                                                           .Include(x => x.Feature)
                                                           .Where(x => x.PlanId == autoRenewal.PlanId)
                                                           .Select(x => new
                                                           {
                                                               x.FeatureId,
                                                               PlanFeatureId = x.Id,
                                                               FeatureReset = x.Feature.Reset,
                                                               x.Limit
                                                           })
                                                           .ToListAsync();

                        subscriptionFeatures = PlanFeatures.Select(x =>
                                                                BuildSubscriptionFeatureEntity(subscription.Id,
                                                                                                x.FeatureId,
                                                                                                x.PlanFeatureId,
                                                                                                x.FeatureReset,
                                                                                                x.Limit)
                                                                       ).ToList();
                    }

                    RenewSubscription(subscription,
                                      subscriptionFeatures,
                                      autoRenewal.PlanId,
                                      autoRenewal.PlanPriceId,
                                      autoRenewal.PlanCycle,
                                      autoRenewal.Price,
                                      autoRenewal.PlanDisplayName);

                    subscription.AddDomainEvent(new SubscriptionRenewedEvent(
                                                        subscription,
                                                        autoRenewal,
                                                        "The Subscription Is Automatically Renewed."));

                    await _dbContext.SaveChangesAsync();
                }

                #endregion

                #region Subscriptions Suspending

                // Setting Expired Subscriptions As Unpaid

                var subscriptionsForSuspending = subscriptions.Where(x => !subscriptionsIdsForAutoRenewal.Contains(x.Id));

                string systemComment = "Setting the Subscription As Unpaid for the tenant due to non-renewal.";

                foreach (var subscription in subscriptionsForSuspending)
                {
                    SetSubscriptionAsUnpaid(subscription, systemComment, _date);

                    subscription.AddDomainEvent(new SubscriptionWasSetAsUnpaidEvent(subscription, systemComment));

                    await _dbContext.SaveChangesAsync();
                }
                #endregion

            }

            return Result.Successful();
        }


        private void RenewSubscription(Subscription subscription,
                                      List<SubscriptionFeature> subscriptionFeatures,
                                      Guid planId,
                                      Guid planPriceId,
                                      PlanCycle planCycle,
                                      decimal planPrice,
                                      string planDisplayName)
        {
            // #1
            var subscriptionCycle = BuildSubscriptionCycleEntity(subscription: subscription,
                                                                 planId: planId,
                                                                 planPriceId: planPriceId,
                                                                 planCycle: planCycle,
                                                                 planPrice: planPrice,
                                                                 planDisplayName: planDisplayName);
            // #2
            UpdateSubscriptionEntity(subscription: subscription,
                                     subscriptionCycleId: subscriptionCycle.Id,
                                     startDate: subscriptionCycle.StartDate,
                                     endDate: subscriptionCycle.EndDate);

            foreach (var subscriptionFeature in subscriptionFeatures)
            {
                SubscriptionFeatureCycle featureCycle = null;

                // #3
                var subscriptionFeatureCycle = BuildSubscriptionFeatureCycleEntity(subscription: subscription,
                                                                                   featureCycle: featureCycle,
                                                                                   subscriptionFeatureId: subscriptionFeature.Id,
                                                                                   subscriptionCycleId: subscriptionCycle.Id);
                // #4
                UpdateSubscriptionFeatureEntity(subscriptionFeature: subscriptionFeature,
                                         subscriptionFeatureCycleId: subscriptionFeatureCycle.Id,
                                         startDate: subscriptionFeatureCycle.StartDate,
                                         endDate: subscriptionFeatureCycle.EndDate,
                                         limit: subscriptionFeatureCycle.Limit);

                _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);
            }



            _dbContext.SubscriptionCycles.Add(subscriptionCycle);
        }

        private SubscriptionFeature BuildSubscriptionFeatureEntity(Guid subscriptionId, Guid featureId, Guid planFeatureId, FeatureReset featureReset, int? limit)
        {
            var subscriptionFeature = new SubscriptionFeature
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscriptionId,
                //SubscriptionFeatureCycleId = f.GeneratedSubscriptionFeatureCycleId,
                StartDate = _date,
                EndDate = FeatureResetManager.FromKey(featureReset).GetExpiryDate(_date),
                FeatureId = featureId,
                PlanFeatureId = planFeatureId,
                RemainingUsage = limit,
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
            };

            return subscriptionFeature;
        }

        private SubscriptionCycle BuildSubscriptionCycleEntity(Subscription subscription, Guid planId, Guid planPriceId, PlanCycle planCycle, decimal planPrice, string planDisplayName)
        {
            var subscriptionCycle = new SubscriptionCycle()
            {
                Id = Guid.NewGuid(),
                StartDate = _date,
                EndDate = PlanCycleManager.FromKey(planCycle).GetExpiryDate(_date),
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
                Price = planPrice,
            };

            return subscriptionCycle;
        }

        private void UpdateSubscriptionEntity(Subscription subscription, Guid subscriptionCycleId, DateTime startDate, DateTime endDate)
        {
            subscription.SubscriptionCycleId = subscriptionCycleId;
            subscription.StartDate = startDate;
            subscription.EndDate = endDate;
        }

        private SubscriptionFeatureCycle BuildSubscriptionFeatureCycleEntity(Subscription subscription, SubscriptionFeatureCycle featureCycle, Guid subscriptionFeatureId, Guid subscriptionCycleId)
        {
            var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
            {
                Id = Guid.NewGuid(),
                StartDate = _date,
                EndDate = FeatureResetManager.FromKey(featureCycle.FeatureReset).GetExpiryDate(_date),
                SubscriptionId = featureCycle.SubscriptionId,
                SubscriptionCycleId = subscriptionCycleId,
                SubscriptionFeatureId = subscriptionFeatureId,
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
                CreatedByUserId = _identityContextService.GetActorId(),
                ModifiedByUserId = _identityContextService.GetActorId(),
                CreationDate = _date,
                ModificationDate = _date,
            };

            return subscriptionFeatureCycle;
        }

        private void UpdateSubscriptionFeatureEntity(SubscriptionFeature subscriptionFeature, Guid subscriptionFeatureCycleId, DateTime? startDate, DateTime? endDate, int? limit)
        {
            subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycleId;
            subscriptionFeature.StartDate = startDate;
            subscriptionFeature.EndDate = endDate;
            subscriptionFeature.RemainingUsage = limit;
        }

        private void SetSubscriptionAsUnpaid(Subscription subscription, string systemComment, DateTime date)
        {
            subscription.IsPaid = false;
            subscription.ModificationDate = date;
            subscription.Comment = systemComment;
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
                        EndDate = PlanCycleManager.FromKey(subscription.PlanPrice.PlanCycle).GetExpiryDate(date),
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
                        SubscriptionId = subscription.Id
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
        #endregion


    }
}

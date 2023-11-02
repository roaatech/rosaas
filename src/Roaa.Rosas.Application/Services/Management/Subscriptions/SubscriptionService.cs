using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Props 
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public SubscriptionService(ILogger<SubscriptionService> logger,
                                   IRosasDbContext dbContext,
                                   IPublisher publisher)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publisher = publisher;
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




        public async Task<Result> SuspendPaymentStatusForSubscriptionDueToNonRenewalAsync(CancellationToken cancellationToken = default)
        {
            string systemComment = "Suspending the payment status for the tenant due to non-renewal of the subscription.";
            var date = DateTime.UtcNow;
            var subscriptions = await _dbContext.Subscriptions
                                                .Where(x => x.StartDate <= date &&
                                                            x.EndDate < date &&
                                                            x.IsPaid &&
                                                            (x.Status == TenantStatus.Active ||
                                                            x.Status == TenantStatus.CreatedAsActive))
                                                .ToListAsync();
            if (subscriptions.Any())
            {
                foreach (var subscription in subscriptions)
                {
                    subscription.IsPaid = false;
                    subscription.ModificationDate = date;
                    subscription.Comment = systemComment;

                }

                subscriptions[0].AddDomainEvent(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SuspendingThePaymentStatusForTenantSubscriptionDueToNonRenewalOfTheSubscription,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: systemComment,
                                                   processId: out _,
                                                   subscriptions: subscriptions));

                await _dbContext.SaveChangesAsync();
            }

            return Result.Successful();
        }

        public async Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default)
        {
            var date = DateTime.UtcNow;
            var subscriptionFeatures = await _dbContext.SubscriptionFeatures
                                                        .Where(x => x.Subscription.StartDate <= date &&
                                                                   x.Subscription.EndDate > date &&
                                                                  x.Subscription.IsPaid &&
                                                                   x.StartDate <= date &&
                                                                  x.EndDate < date)
                                                       .ToListAsync();

            if (subscriptionFeatures.Any())
            {
                var cyclesIds = subscriptionFeatures.Select(x => x.SubscriptionFeatureCycleId).ToList();

                var cycles = await _dbContext.SubscriptionFeatureCycles
                                                           .Where(x => cyclesIds.Contains(x.Id))
                                                           .ToListAsync();

                foreach (var subscriptionFeature in subscriptionFeatures)
                {
                    var cycle = cycles.Where(x => x.Id == subscriptionFeature.SubscriptionFeatureCycleId).FirstOrDefault();

                    var subscriptionFeatureCycle = new SubscriptionFeatureCycle()
                    {
                        Id = Guid.NewGuid(),
                        StartDate = date,
                        EndDate = FeatureResetManager.FromKey(cycle.Reset).GetExpiryDate(date),
                        SubscriptionId = cycle.SubscriptionId,
                        SubscriptionCycleId = cycle.SubscriptionCycleId,
                        SubscriptionFeatureId = subscriptionFeature.Id,
                        FeatureId = cycle.FeatureId,
                        PlanFeatureId = cycle.PlanFeatureId,
                        Limit = cycle.Limit,
                        Reset = cycle.Reset,
                        Type = cycle.Type,
                        Unit = cycle.Unit,
                        TotalUsage = cycle.Limit is null ? null : 0,
                        RemainingUsage = cycle.Limit,
                        Cycle = cycle.Cycle,
                        FeatureName = cycle.FeatureName,
                        CreatedByUserId = cycle.CreatedByUserId,
                        ModifiedByUserId = cycle.ModifiedByUserId,
                        CreationDate = date,
                        ModificationDate = date,
                    };

                    subscriptionFeature.SubscriptionFeatureCycleId = subscriptionFeatureCycle.Id;
                    subscriptionFeature.StartDate = subscriptionFeatureCycle.StartDate;
                    subscriptionFeature.EndDate = subscriptionFeatureCycle.EndDate;
                    subscriptionFeature.RemainingUsage = subscriptionFeatureCycle.Limit;

                    _dbContext.SubscriptionFeatureCycles.Add(subscriptionFeatureCycle);
                }

                await _dbContext.SaveChangesAsync();
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
                        EndDate = PlanCycleManager.FromKey(subscription.PlanPrice.Cycle).GetExpiryDate(date),
                        TenantId = subscription.TenantId,
                        PlanId = subscription.PlanId,
                        PlanPriceId = subscription.PlanPriceId,
                        ProductId = subscription.ProductId,
                        Cycle = subscription.PlanPrice.Cycle,
                        PlanName = subscription.Plan.Name,
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
                                EndDate = FeatureResetManager.FromKey(featureCycle.Reset).GetExpiryDate(date),
                                SubscriptionId = featureCycle.SubscriptionId,
                                SubscriptionCycleId = subscriptionCycle.Id,
                                SubscriptionFeatureId = subscriptionFeature.Id,
                                FeatureId = featureCycle.FeatureId,
                                PlanFeatureId = featureCycle.PlanFeatureId,
                                Limit = featureCycle.Limit,
                                Reset = featureCycle.Reset,
                                Type = featureCycle.Type,
                                Unit = featureCycle.Unit,
                                TotalUsage = featureCycle.Limit is null ? null : 0,
                                RemainingUsage = featureCycle.Limit,
                                Cycle = featureCycle.Cycle,
                                FeatureName = featureCycle.FeatureName,
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

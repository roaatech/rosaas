using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Props 
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly ISender _mediator;
        #endregion


        #region Corts
        public SubscriptionService(ILogger<SubscriptionService> logger,
                                   IRosasDbContext dbContext,
                                   ISender mediator)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mediator = mediator;
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
                await _mediator.Send(new ChangeTenantStatusByIdCommand(subscription.TenantId,
                                                                       TenantStatus.PreDeactivating,
                                                                       subscription.ProductId,
                                                                       "Note By System: Deactivating the tenant due to non-payment of the subscription."),
                                    cancellationToken);

            }
            return Result.Successful();
        }




        public async Task<Result> SuspendPaymentStatusForSubscriptionDueToNonRenewalAsync(CancellationToken cancellationToken = default)
        {
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
                    subscription.Notes = "Note By System: Suspending the payment status for the tenant due to non-renewal of the subscription.";

                    var processHistory = new TenantProcessHistory
                    {
                        Id = Guid.NewGuid(),
                        TenantId = subscription.TenantId,
                        ProductId = subscription.ProductId,
                        SubscriptionId = subscription.Id,
                        Status = subscription.Status,
                        OwnerType = UserType.RosasSystem,
                        ProcessDate = date,
                        TimeStamp = date,
                        ProcessType = TenantProcessType.SuspendingThePaymentStatusForTheSubscriberDueToNonRenewalOfTheSubscription,
                        Enabled = true,
                        Notes = subscription.Notes
                    };
                    _dbContext.TenantProcessHistory.Add(processHistory);
                }
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
        #endregion


    }
}

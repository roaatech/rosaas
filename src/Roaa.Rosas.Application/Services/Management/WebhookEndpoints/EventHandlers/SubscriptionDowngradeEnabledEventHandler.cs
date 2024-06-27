using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionDowngradeEnabledEventHandler : BaseWebhookEventHandler<SubscriptionDowngradeEnabledEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.DowngradeEnabled;



        public SubscriptionDowngradeEnabledEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = await DbContext!.Subscriptions.Where(x => x.Id == Event!.SubscriptionId)
                                                            .Select(x => new { x.ProductId, x.Tenant!.SystemName })
                                                            .SingleOrDefaultAsync(cancellationToken);
            var downgradedPlan = await DbContext!.Plans
                         .Where(plan => plan.Id == Event!.NewPlanId)
                         .Select(plan => new { plan.SystemName })
                         .SingleOrDefaultAsync(cancellationToken);

            var downgradedPlanPrice = await DbContext!.PlanPrices
                                                 .Where(planPrice => planPrice.Id == Event!.NewPlanPriceId)
                                                 .Select(planPrice => new { price = planPrice.Price, planCycle = planPrice.PlanCycle })
                                                 .SingleOrDefaultAsync(cancellationToken);

            var metadata = new
            {
                downgradeEnabled = true,
                downgradedPlanPrice = downgradedPlanPrice!.price,
                downgradedPlanCycle = downgradedPlanPrice.planCycle,
                downgradedPlanSystemName = downgradedPlan!.SystemName,

                downgradeForced = false
            };

            return (metadata, subscription!.ProductId, subscription.SystemName);
        }
    }
}

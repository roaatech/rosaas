using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionUpgradeEnabledEventHandler : BaseWebhookEventHandler<SubscriptionUpgradeEnabledEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.UpgradeEnabled;



        public SubscriptionUpgradeEnabledEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = await DbContext!.Subscriptions
                                                            .Where(x => x.Id == Event!.SubscriptionId)
                                                            .Select(x => new { x.ProductId, x.Tenant!.SystemName, x.Plan })
                                                            .SingleOrDefaultAsync(cancellationToken);
            var upgradedPlan = await DbContext!.Plans
                                                 .Where(plan => plan.Id == Event!.NewPlanId)
                                                 .Select(plan => new { plan.SystemName })
                                                 .SingleOrDefaultAsync(cancellationToken);

            var upgradedPlanPrice = await DbContext!.PlanPrices
                                                 .Where(planPrice => planPrice.Id == Event!.NewPlanPriceId)
                                                 .Select(planPrice => new { price = planPrice.Price, planCycle = planPrice.PlanCycle })
                                                 .SingleOrDefaultAsync(cancellationToken);
            var metadata = new
            {
                upgradeEnabled = true,
                upgradedPlansystemName = upgradedPlan!.SystemName,
                upgradedPlanPrice = upgradedPlanPrice!.price,
                upgradedPlanCycle = upgradedPlanPrice!.planCycle,
                previousPlan = subscription!.Plan!.SystemName
            };

            return (metadata, subscription!.ProductId, subscription.SystemName);
        }
    }

}

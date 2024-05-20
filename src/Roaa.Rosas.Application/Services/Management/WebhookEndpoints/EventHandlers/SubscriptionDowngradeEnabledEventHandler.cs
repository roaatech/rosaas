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
            var metadata = new
            {
                downgradeEnabled = true,
                downgradedPlanId = Event!.NewPlanId,
                downgradedPlanPriceId = Event!.NewPlanPriceId,
                downgradeForced = false
            };

            return (metadata, subscription!.ProductId, subscription.SystemName);
        }
    } 
}

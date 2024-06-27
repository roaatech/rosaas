using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionHasBeenDowngradedEventHandler : BaseWebhookEventHandler<SubscriptionHasBeenDowngradedEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.SubscriptionHasBeenDowngraded;



        public SubscriptionHasBeenDowngradedEventHandler(IServiceScopeFactory serviceScopeFactory)
                 : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var tenantSystemName = await DbContext!.Tenants.Where(x => x.Id == Event!.Subscription.TenantId)
                                                         .Select(x => x.SystemName)
                                                         .SingleOrDefaultAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(tenantSystemName);

            var metadata = new
            {
                SubscriptionExpirationDate = Event!.Subscription.EndDate,
                Date = DateTime.UtcNow,
                SubscriptionNewPlan = Event!.Subscription.Plan!.DisplayName,
            };

            return (metadata, Event!.Subscription.ProductId, tenantSystemName);
        }
    }
}
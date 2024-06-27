using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class TenantCreatedInStoreEventHandler : BaseWebhookEventHandler<TenantCreatedInStoreEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.TenantRegisteredInRoSaasDb;



        public TenantCreatedInStoreEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = Event!.Tenant.Subscriptions!.First();
            var metadata = new
            {
                creationDate = subscription.CreationDate,
                endDate = subscription.EndDate,
                Plan = subscription.Plan!.SystemName,
                planPrice = subscription.PlanPrice!.Price,
                Date = DateTime.UtcNow,
            };
            return (metadata, subscription.ProductId, Event!.Tenant.SystemName);
        }
    }
}
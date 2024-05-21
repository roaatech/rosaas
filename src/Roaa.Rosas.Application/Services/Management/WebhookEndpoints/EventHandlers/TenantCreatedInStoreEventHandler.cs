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
            var metadata = new
            {
                Date = DateTime.UtcNow,
            };
            return (metadata, Event!.Tenant.Subscriptions!.First().ProductId, Event!.Tenant.SystemName);
        }
    }
}

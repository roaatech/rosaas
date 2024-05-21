using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class ExternalSystemCreatedTenantResourcesEventHandler : BaseWebhookEventHandler<ExternalSystemCreatedTenantResourcesEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.ExternalSystemCreatedTenantResources;



        public ExternalSystemCreatedTenantResourcesEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }



        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var productId = await DbContext!.Subscriptions
                                         .Where(x => Event!.TenantSystemName.ToLower().Equals(x.Tenant!.SystemName))
                                         .Select(x => x.ProductId)
                                         .FirstOrDefaultAsync(cancellationToken);


            var metadata = new
            {
                Date = DateTime.UtcNow,
            };

            return (metadata, productId, Event!.TenantSystemName);
        }
    }
}

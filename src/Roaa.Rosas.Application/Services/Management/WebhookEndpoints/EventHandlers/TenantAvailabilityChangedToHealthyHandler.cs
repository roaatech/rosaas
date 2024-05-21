using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class TenantAvailabilityChangedToHealthyHandler : BaseWebhookEventHandler<TenantAvailabilityChangedToHealthyEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.TenantAvailabilityChangedToHealthy;



        public TenantAvailabilityChangedToHealthyHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }



        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var tenantSystemName = await DbContext!.Tenants.Where(x => x.Id == Event!.TenantId)
                                                          .Select(x => x.SystemName)
                                                          .SingleOrDefaultAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(tenantSystemName);

            var metadata = new
            {
                Date = DateTime.UtcNow,
            };

            return (metadata, Event!.ProductId, tenantSystemName);
        }
    }
}

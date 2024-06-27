using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionSuspendedDueToUnpaidEventHandler : BaseWebhookEventHandler<SubscriptionWasSetAsInactiveDueToUnpaideEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.SubscriptionSuspendedDueToUnpaid;



        public SubscriptionSuspendedDueToUnpaidEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }



        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var tenantSystemName = await DbContext!.Tenants.Where(x => x.Id == Event!.Subscription.TenantId)
                                                          .Select(x => x.SystemName)
                                                          .SingleOrDefaultAsync(cancellationToken);

            ArgumentNullException.ThrowIfNull(tenantSystemName);

            var metadata = new
            {
                Date = DateTime.UtcNow,
                Plan = Event!.Subscription.Plan!.SystemName,
                PlanPrice = Event!.Subscription.PlanPrice!.Price,
                plancycle = Event!.Subscription.PlanPrice!.PlanCycle,
            };

            return (metadata, Event!.Subscription.ProductId, tenantSystemName);
        }
    }
}

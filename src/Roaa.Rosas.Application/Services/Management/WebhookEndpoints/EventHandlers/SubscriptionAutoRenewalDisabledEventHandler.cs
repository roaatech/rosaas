using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalDisabledEventHandler : BaseWebhookEventHandler<SubscriptionAutoRenewalDisabledEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.SubscriptionAutoRenewalDisabled;



        public SubscriptionAutoRenewalDisabledEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = await DbContext!.Subscriptions.Where(x => x.Id == Event!.SubscriptionRenewal.SubscriptionId)
                                                            .Select(x => new { x.ProductId, x.Tenant!.SystemName, x.Plan })
                                                            .SingleOrDefaultAsync(cancellationToken);

            var metadata = new
            {
                CurrentSubscriptionPlan = subscription!.Plan!.SystemName,
                CancellationTime = DateTime.UtcNow,
            };
            return (metadata, subscription!.ProductId, subscription.SystemName);
        }
    }
}

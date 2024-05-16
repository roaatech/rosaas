using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalDisabledEventHandler : BaseWebhookEventHandler<SubscriptionAutorenewalDisabledEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.AutoRenewalCanceled;



        public SubscriptionAutoRenewalDisabledEventHandler(IServiceScopeFactory serviceScopeFactory)
                : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = await DbContext!.Subscriptions.Where(x => x.Id == Event!.SubscriptionRenewal.SubscriptionId)
                                                            .Select(x => new { x.ProductId, x.Tenant!.SystemName })
                                                            .SingleOrDefaultAsync(cancellationToken);

            return (null, subscription!.ProductId, subscription.SystemName);
        }
    }
}

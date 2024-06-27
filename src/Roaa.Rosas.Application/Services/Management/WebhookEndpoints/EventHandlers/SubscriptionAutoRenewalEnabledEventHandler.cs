using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalEnabledEventHandler : BaseWebhookEventHandler<SubscriptionAutorenewalEnabledEvent, dynamic>
    {
        protected override WebhookEvents EventType => WebhookEvents.AutoRenewalEnabled;



        public SubscriptionAutoRenewalEnabledEventHandler(IServiceScopeFactory serviceScopeFactory)
                 : base(serviceScopeFactory) { }




        protected async override Task<(dynamic metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default)
        {
            var subscription = await DbContext!.Subscriptions.Where(x => x.Id == Event!.SubscriptionRenewal.SubscriptionId)
                                                            .Select(x => new { x.ProductId, x.Tenant!.SystemName })
                                                            .SingleOrDefaultAsync(cancellationToken);

            var metadata = new
            {
                autoRenewalEnabled = true,
                renewalsCount = Event!.SubscriptionRenewal.RenewalsCount,
                isContinuousRenewal = Event.SubscriptionRenewal.IsContinuousRenewal,
                renewalCycle = Event!.SubscriptionRenewal.PlanCycle
            };

            return (metadata, subscription!.ProductId, subscription.SystemName);
        }
    }
}
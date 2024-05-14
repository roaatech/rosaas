using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalDisabledEventHandler : BaseWebhookEventHandler<SubscriptionAutorenewalDisabledEvent>
    {
        private readonly IWebhookAPI _webhookAPI;

        public SubscriptionAutoRenewalDisabledEventHandler(IRosasDbContext dbContext, IWebhookAPI webhookAPI) : base(dbContext)
        {
            _webhookAPI = webhookAPI;
        }

        protected override WebhookEvents EventType => WebhookEvents.AutoRenewalCanceled;

        public override async Task Handle(SubscriptionAutorenewalDisabledEvent @event, CancellationToken cancellationToken = default)
        {
            var callerModel = await BuildModel(@event.SubscriptionRenewal.Subscription!.Tenant!.Id, @event, new { autoRenewal = true }, @event.SubscriptionRenewal.Subscription!.Tenant!.SystemName, cancellationToken);
            await _webhookAPI.CallWebhookEndpointsAsync(callerModel, cancellationToken);
        }

    }
}

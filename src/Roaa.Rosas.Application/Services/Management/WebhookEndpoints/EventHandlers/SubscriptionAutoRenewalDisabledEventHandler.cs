using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalDisabledEventHandler : BaseWebhookEventHandler<SubscriptionAutorenewalDisabledEvent>
    {
        private readonly IWebhookAPI _webhookAPI;
        private readonly IRosasDbContext _dbContext;

        public SubscriptionAutoRenewalDisabledEventHandler(IRosasDbContext dbContext, IWebhookAPI webhookAPI) : base(dbContext)
        {
            _webhookAPI = webhookAPI;
            _dbContext = dbContext;

        }

        protected override WebhookEvents EventType => WebhookEvents.AutoRenewalCanceled;

        public override async Task Handle(SubscriptionAutorenewalDisabledEvent @event, CancellationToken cancellationToken = default)
        {
            var sub = await _dbContext.Subscriptions.Where(x => x.Id == @event.SubscriptionRenewal.SubscriptionId)
                                                     .Select(x => new { x.ProductId, x.Tenant.SystemName })
                                                     .SingleOrDefaultAsync(cancellationToken);

            var callerModel = await BuildModel(sub.ProductId,
                                                @event,
                                                new { autoRenewal = false },
                                                sub.SystemName,
                                                cancellationToken);
            await _webhookAPI.CallWebhookEndpointsAsync(callerModel, cancellationToken);
        }

    }
}

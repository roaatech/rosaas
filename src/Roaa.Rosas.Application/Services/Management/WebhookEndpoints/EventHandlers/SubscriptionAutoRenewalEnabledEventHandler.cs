using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models.Webhook;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public class SubscriptionAutoRenewalEnabledEventHandler : IInternalDomainEventHandler<SubscriptionAutorenewalEnabledEvent>
    {
        private readonly IRosasDbContext _dbContext;
        private readonly IWebhookAPI _webhookAPI;

        protected WebhookEvents EventType { get; set; } = WebhookEvents.AutoRenewalEnabled;



        public SubscriptionAutoRenewalEnabledEventHandler(IRosasDbContext dbContext, IWebhookAPI webhookAPI)
        {
            _dbContext = dbContext;
            _webhookAPI = webhookAPI;
        }



        public async Task Handle(SubscriptionAutorenewalEnabledEvent @event, CancellationToken cancellationToken = default)
        {
            var sub = await _dbContext.Subscriptions.Where(x => x.Id == @event.SubscriptionRenewal.SubscriptionId)
                 .Select(x => new { x.ProductId, x.Tenant.SystemName })
                  .SingleOrDefaultAsync(cancellationToken);


            var callerModel = await BuildModel(sub.ProductId,
                                                new { autoRenewal = true },
                                                sub.SystemName,
                                                cancellationToken);

            await _webhookAPI.CallWebhookEndpointsAsync(callerModel, cancellationToken);
        }




        private async Task<List<WebhookEndpointModel>> GetWebhookEndpointsAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.WebhookEndpointEvents
                                    .Where(x => x.Event == EventType && x.WebhookEndpoint!.EntityId == entityId)
                                    .Select(x => new WebhookEndpointModel
                                    {
                                        Url = x.WebhookEndpoint!.Url,
                                        Secret = x.WebhookEndpoint.SigningSecret
                                    })
                .ToListAsync(cancellationToken);
        }

        private async Task<WebhookCallingModel<GlobalPayload<T>>> BuildModel<T>(Guid productId, T metadata, string tenantSystemName, CancellationToken cancellationToken = default)
        {
            var webhookEndpoints = await GetWebhookEndpointsAsync(productId, cancellationToken);
            return new WebhookCallingModel<GlobalPayload<T>>
            {
                Payload = new GlobalPayload<T>
                {
                    EventType = EventType,
                    TenantSystemName = tenantSystemName,
                    MetaData = metadata
                },
                WebhookEndpoints = webhookEndpoints
            };
        }
    }

}
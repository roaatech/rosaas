using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Webhook;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public abstract class BaseWebhookEventHandler<TEvent> : IInternalDomainEventHandler<TEvent> where TEvent : BaseInternalEvent
    {
        private readonly IRosasDbContext _dbContext;

        protected abstract WebhookEvents EventType { get; }

        protected BaseWebhookEventHandler(IRosasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected async Task<List<WebhookEndpointModel>> GetWebhookEndpointsAsync(Guid entityId, CancellationToken cancellationToken = default)
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

        protected async Task<WebhookCallingModel<GlobalPayload<T>>> BuildModel<T>(Guid entityId, TEvent @event, T metadata, string tenantSystemName, CancellationToken cancellationToken = default)
        {
            var webhookEndpoints = await GetWebhookEndpointsAsync(entityId, cancellationToken);
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

        public abstract Task Handle(TEvent @event, CancellationToken cancellationToken = default);
    }

}

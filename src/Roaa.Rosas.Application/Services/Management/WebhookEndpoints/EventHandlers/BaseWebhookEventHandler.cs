using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Webhook;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    public abstract class BaseWebhookEventHandler<TEvent, TPayloadMetadata> : IInternalDomainEventHandler<TEvent> where TEvent : BaseInternalEvent
    {
        #region Props

        protected readonly IServiceScopeFactory ServiceScopeFactory;
        protected IRosasDbContext? DbContext { get; private set; }
        protected IWebhookAPI? WebhookAPI { get; private set; }
        protected TEvent? Event { get; private set; } = null;
        private Guid _productId { get; set; }
        private TPayloadMetadata? _metadata { get; set; } = default;
        private string _tenantSystemName { get; set; } = string.Empty;
        #endregion



        #region Ctrs
        protected BaseWebhookEventHandler(IServiceScopeFactory serviceScopeFactory)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
        }
        #endregion



        #region abstractions
        protected abstract WebhookEvents EventType { get; }
        protected abstract Task<(TPayloadMetadata metadata, Guid productId, string tenantSystemName)> PreparePayloadAsync(CancellationToken cancellationToken = default);
        #endregion






        #region utilities
        public virtual async Task Handle(TEvent @event, CancellationToken cancellationToken = default)
        {
            _ = Task.Run(async () => await HandleWebhookAsync(@event, cancellationToken));

        }

        public virtual async Task HandleWebhookAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = ServiceScopeFactory.CreateScope();
                DbContext = scope.ServiceProvider.GetRequiredService<IRosasDbContext>();
                WebhookAPI = scope.ServiceProvider.GetRequiredService<IWebhookAPI>();



                Event = @event;

                (_metadata, _productId, _tenantSystemName) = await PreparePayloadAsync(cancellationToken);

                var callerModel = await BuildWebhookCallingModel(cancellationToken);

                await WebhookAPI.CallWebhookEndpointsAsync(callerModel, cancellationToken);

            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error calling webhook: {ex.Message}");
            }
        }

        protected virtual async Task<WebhookCallingModel<GlobalPayload<TPayloadMetadata>>> BuildWebhookCallingModel(CancellationToken cancellationToken = default)
        {
            return new WebhookCallingModel<GlobalPayload<TPayloadMetadata>>
            {
                Payload = new GlobalPayload<TPayloadMetadata>
                {
                    EventType = EventType,
                    TenantSystemName = _tenantSystemName,
                    MetaData = _metadata,
                },
                WebhookEndpoints = await GetWebhookEndpointsAsync(cancellationToken)
            };
        }

        protected virtual async Task<List<WebhookEndpointModel>> GetWebhookEndpointsAsync(CancellationToken cancellationToken = default)
        {
            return await DbContext!.WebhookEndpointEvents
                                    .Where(x => x.Event == EventType &&
                                                x.WebhookEndpoint!.EntityId == _productId &&
                                                x.WebhookEndpoint.EntityType == Common.Enums.EntityType.Product)
                                    .Select(x => new WebhookEndpointModel
                                    {
                                        Url = x.WebhookEndpoint!.Url,
                                        Secret = x.WebhookEndpoint.SigningSecret
                                    })
                                    .ToListAsync(cancellationToken);
        }

        #endregion
    }
}

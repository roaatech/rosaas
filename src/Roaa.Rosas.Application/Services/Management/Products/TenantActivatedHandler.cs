using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Handlers
{
    public class ProductUpdatedHandler : IInternalDomainEventHandler<ProductUpdatedEvent>
    {
        private readonly ILogger<ProductUpdatedHandler> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;

        public ProductUpdatedHandler(ILogger<ProductUpdatedHandler> logger,
                                     BackgroundServicesStore backgroundWorkerStore)
        {
            _backgroundWorkerStore = backgroundWorkerStore;
            _logger = logger;
        }

        public async Task Handle(ProductUpdatedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                if (!@event.OldProduct.ApiKey.Equals(@event.UpdatedProduct.ApiKey))
                {
                    _backgroundWorkerStore.RemoveProductAPIKey(@event.UpdatedProduct.Id);

                    _backgroundWorkerStore.AddProductAPIKey(@event.UpdatedProduct.Id, @event.UpdatedProduct.ApiKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred on {0} while processing the Event Handler related to product [ProductId:{2}]",
                                    GetType().Name,
                                    @event.UpdatedProduct.Id);
            }

        }
    }
}


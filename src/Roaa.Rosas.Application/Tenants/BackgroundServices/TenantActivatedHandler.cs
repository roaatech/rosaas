using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices.Workers
{
    public class TenantActivatedHandler : IInternalDomainEventHandler<TenantActivatedEvent>
    {
        private readonly ILogger<TenantActivatedHandler> _logger;
        private readonly BackgroundWorkerStore _backgroundWorkerStore;

        public TenantActivatedHandler(ILogger<TenantActivatedHandler> logger,
                                                         BackgroundWorkerStore backgroundWorkerStore)
        {
            _backgroundWorkerStore = backgroundWorkerStore;
            _logger = logger;
        }

        public async Task Handle(TenantActivatedEvent @event, CancellationToken cancellationToken)
        {
            _backgroundWorkerStore.AddAvailableTenantTask(
                new JobTask
                {
                    Id = Guid.NewGuid(),
                    ProductId = @event.ProductTenant.ProductId,
                    TenantId = @event.ProductTenant.TenantId,
                    Created = DateTime.UtcNow,
                    Type = JobTaskType.Available,
                });

            _logger.LogInformation($"The job task added to {nameof(AvailableTenantChecker)} Background Service with info: TenantId:{{0}}, ProductId:{{1}}",
                  @event.ProductTenant.TenantId,
                  @event.ProductTenant.ProductId);

        }
    }
}


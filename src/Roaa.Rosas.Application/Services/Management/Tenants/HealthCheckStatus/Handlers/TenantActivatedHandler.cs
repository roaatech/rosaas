using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Handlers
{
    public class TenantActivatedHandler : IInternalDomainEventHandler<TenantActivatedEvent>
    {
        private readonly ILogger<TenantActivatedHandler> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;
        private readonly IRosasDbContext _dbContext;

        public TenantActivatedHandler(ILogger<TenantActivatedHandler> logger,
                                      IRosasDbContext dbContext,
                                      BackgroundServicesStore backgroundWorkerStore)
        {
            _backgroundWorkerStore = backgroundWorkerStore;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(TenantActivatedEvent @event, CancellationToken cancellationToken)
        {
            var tenantName = await _dbContext.Tenants
                           .Where(x => x.Id == @event.ProductTenant.TenantId)
                           .Select(x => x.UniqueName)
                           .SingleOrDefaultAsync(cancellationToken);

            _backgroundWorkerStore.AddAvailableTenantTask(
                new JobTask
                {
                    Id = Guid.NewGuid(),
                    ProductId = @event.ProductTenant.ProductId,
                    TenantId = @event.ProductTenant.TenantId,
                    Created = DateTime.UtcNow,
                    Type = JobTaskType.Available,
                }, tenantName);

            _logger.LogInformation($"The job task added to {nameof(AvailableTenantChecker)} Background Service with info: TenantId:{{0}}, ProductId:{{1}}",
                  @event.ProductTenant.TenantId,
                  @event.ProductTenant.ProductId);

        }
    }
}


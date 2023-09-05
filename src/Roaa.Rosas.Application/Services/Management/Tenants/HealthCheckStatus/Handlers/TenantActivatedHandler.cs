using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Services;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Handlers
{
    public class TenantActivatedHandler : IInternalDomainEventHandler<TenantActivatedEvent>
    {
        private readonly ILogger<TenantActivatedHandler> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;
        private readonly IRosasDbContext _dbContext;
        private readonly ITenantHealthCheckService _tenantHealthCheckService;

        public TenantActivatedHandler(ILogger<TenantActivatedHandler> logger,
                                      IRosasDbContext dbContext,
                                      BackgroundServicesStore backgroundWorkerStore,
                                      ITenantHealthCheckService tenantHealthCheckService)
        {
            _backgroundWorkerStore = backgroundWorkerStore;
            _tenantHealthCheckService = tenantHealthCheckService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(TenantActivatedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                var date = DateTime.UtcNow;
                var jobTask = new JobTask
                {
                    Id = Guid.NewGuid(),
                    SubscriptionId = @event.Subscription.Id,
                    ProductId = @event.Subscription.ProductId,
                    TenantId = @event.Subscription.TenantId,
                    Created = date,
                    Type = JobTaskType.Available,
                };

                await _tenantHealthCheckService.ResetTenantHealthStatusCountersAsync(jobTask, cancellationToken);

                var tenantName = await _dbContext.Tenants
                               .Where(x => x.Id == @event.Subscription.TenantId)
                               .Select(x => x.UniqueName)
                               .SingleOrDefaultAsync(cancellationToken);

                _backgroundWorkerStore.AddAvailableTenantTask(jobTask, tenantName);

                _logger.LogInformation($"The job task added to {nameof(AvailableTenantChecker)} Background Service with info: TenantId:{{0}}, ProductId:{{1}}",
                      @event.Subscription.TenantId,
                      @event.Subscription.ProductId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred on {0} while processing the Event Handler related to the tenant [TenantId:{1}], [ProductId:{2}]", GetType().Name, @event.Subscription.TenantId, @event.Subscription.ProductId);
            }

        }
    }
}


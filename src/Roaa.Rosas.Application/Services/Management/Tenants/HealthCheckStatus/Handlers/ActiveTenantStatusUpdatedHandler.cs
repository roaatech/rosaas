using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Handlers
{
    public class ActiveTenantStatusUpdatedHandler : IInternalDomainEventHandler<ActiveTenantStatusUpdated>
    {
        private readonly ILogger<ActiveTenantStatusUpdatedHandler> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;
        private readonly IRosasDbContext _dbContext;

        public ActiveTenantStatusUpdatedHandler(ILogger<ActiveTenantStatusUpdatedHandler> logger,
                                                         BackgroundServicesStore backgroundWorkerStore,
                                                         IRosasDbContext dbContext)
        {
            _logger = logger;
            _backgroundWorkerStore = backgroundWorkerStore;
            _dbContext = dbContext;
        }

        public async Task Handle(ActiveTenantStatusUpdated @event, CancellationToken cancellationToken)
        {
            try
            {
                var jobTasksToRemove = await _dbContext.JobTasks
                                                       .Where(x => x.TenantId == @event.ProductTenant.TenantId &&
                                                                   x.ProductId == @event.ProductTenant.ProductId)
                                                       .ToListAsync(cancellationToken);
                if (jobTasksToRemove.Any())
                {
                    _dbContext.JobTasks.RemoveRange(jobTasksToRemove);

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                _backgroundWorkerStore.RemoveJobTask(new JobTask
                {
                    ProductId = @event.ProductTenant.ProductId,
                    TenantId = @event.ProductTenant.TenantId,
                    Created = DateTime.UtcNow,
                    Type = JobTaskType.Available,
                });

                _logger.LogInformation($"The job tasks removed from Background Services with info: TenantId:{{0}}, ProductId:{{1}}",
                  @event.ProductTenant.TenantId,
                  @event.ProductTenant.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred on {0} while processing the Event Handler related to the tenant [TenantId:{1}], [ProductId:{2}]", GetType().Name, @event.ProductTenant.TenantId, @event.ProductTenant.ProductId);
            }

        }
    }
}


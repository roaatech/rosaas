using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.BackgroundServices.Workers;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices
{

    public class BackgroundServiceManager
    {
        private readonly ILogger<BackgroundServiceManager> _logger;
        private readonly BackgroundWorkerStore _store;
        protected readonly IServiceScopeFactory _serviceScopeFactory;


        public BackgroundServiceManager(ILogger<BackgroundServiceManager> logger,
                                 BackgroundWorkerStore store,
                                 IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _store = store;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task PrepareAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IRosasDbContext>();

            var activeTenants = await dbContext
                                    .ProductTenants
                                    .Where(x => x.Status == TenantStatus.Active ||
                                                x.Status == TenantStatus.CreatedAsActive)
                                    .OrderBy(x => x.Edited)
                                    .Select(x => new { x.ProductId, x.TenantId })
                                    .ToListAsync();

            var tasks = await dbContext
                                     .JobTasks
                                     .OrderBy(x => x.Created)
                                     .ToListAsync();




            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
                activeTenants.Count,
                JobTaskType.Available,
                nameof(AvailableTenantChecker));

            activeTenants.ForEach(task => _store.AddAvailableTenantTask(new JobTask
            {
                Id = Guid.NewGuid(),
                ProductId = task.ProductId,
                TenantId = task.TenantId,
                Created = DateTime.UtcNow,
                Type = JobTaskType.Available,
            }));





            var unavailabeTasks = tasks.Where(task => task.Type == JobTaskType.Unavailable &&
                                                     !activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                             x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              unavailabeTasks.Count,
              JobTaskType.Unavailable,
              nameof(UnavailableTenantChecker));

            unavailabeTasks.ForEach(task => _store.AddUnavailableTenantTask(task));







            var inaccessibleTasks = tasks.Where(task => task.Type == JobTaskType.Inaccessible &&
                                                       !activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                               x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              inaccessibleTasks.Count,
              JobTaskType.Inaccessible,
              nameof(InaccessibleTenantChecker));

            inaccessibleTasks.ForEach(task => _store.AddInaccessibleTenantTask(task));







            var informerTasks = tasks.Where(task => task.Type == JobTaskType.Informer &&
                                                       !activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                               x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              informerTasks.Count,
              JobTaskType.Informer,
              nameof(Informer));

            informerTasks.ForEach(task => _store.AddInformerTask(task));



            var entityType = dbContext.Model.FindEntityType(typeof(ProductTenantHealthStatus));
            var schema = entityType.GetSchema();
            _store.ProductTenantHealthStatusTableName = entityType.GetTableName();
        }
    }
}

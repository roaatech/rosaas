using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Settings;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus
{

    public class BackgroundServiceManager
    {
        private readonly ILogger<BackgroundServiceManager> _logger;
        private readonly BackgroundServicesStore _store;
        protected readonly IRosasDbContext _dbContext;
        protected readonly ISettingService _settingService;


        public BackgroundServiceManager(ILogger<BackgroundServiceManager> logger,
                                 BackgroundServicesStore store,
                                  IRosasDbContext dbContext,
                                  ISettingService settingService)
        {
            _logger = logger;
            _store = store;
            _dbContext = dbContext;
            _settingService = settingService;
        }


        public async Task PrepareAsync()
        {

            var activeTenants = await _dbContext
                                    .ProductTenants
                                    .Where(x => x.Status == TenantStatus.Active ||
                                                x.Status == TenantStatus.CreatedAsActive)
                                    .OrderBy(x => x.Edited)
                                    .Select(x => new { x.ProductId, x.TenantId, x.Tenant.UniqueName })
                                    .ToListAsync();


            activeTenants.ForEach(x => _store.AddTenantsNames(x.TenantId, x.UniqueName));

            var tasks = await _dbContext
                                     .JobTasks
                                     .OrderBy(x => x.Created)
                                     .ToListAsync();











            var unavailabeTasks = tasks.Where(task => task.Type == JobTaskType.Unavailable &&
                                                      activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                             x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              unavailabeTasks.Count,
              JobTaskType.Unavailable,
              nameof(UnavailableTenantChecker));

            unavailabeTasks.ForEach(task => _store.AddUnavailableTenantTask(task));







            var inaccessibleTasks = tasks.Where(task => task.Type == JobTaskType.Inaccessible &&
                                                        activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                               x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              inaccessibleTasks.Count,
              JobTaskType.Inaccessible,
              nameof(InaccessibleTenantChecker));

            inaccessibleTasks.ForEach(task => _store.AddInaccessibleTenantTask(task));







            var informerTasks = tasks.Where(task => task.Type == JobTaskType.Informer &&
                                                        activeTenants.Any(x => x.TenantId == task.TenantId &&
                                                                               x.ProductId == task.ProductId))
                                        .ToList();

            _logger.LogInformation("There are [{0}] {1} job tasks added to {2} Background Service.",
              informerTasks.Count,
              JobTaskType.Informer,
              nameof(Informer));

            informerTasks.ForEach(task => _store.AddInformerTask(task));


            foreach (var task in unavailabeTasks)
            {
                var activeTenant = activeTenants.Where(x => task.TenantId == x.TenantId && task.ProductId == x.ProductId).FirstOrDefault();
                if (activeTenant is not null)
                {
                    activeTenants.Remove(activeTenant);
                }
            }

            foreach (var task in inaccessibleTasks)
            {
                var activeTenant = activeTenants.Where(x => task.TenantId == x.TenantId && task.ProductId == x.ProductId).FirstOrDefault();
                if (activeTenant is not null)
                {
                    activeTenants.Remove(activeTenant);
                }
            }

            foreach (var task in informerTasks)
            {
                var activeTenant = activeTenants.Where(x => task.TenantId == x.TenantId && task.ProductId == x.ProductId).FirstOrDefault();
                if (activeTenant is not null)
                {
                    activeTenants.Remove(activeTenant);
                }
            }



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
            }, task.UniqueName));









            var entityType = _dbContext.Model.FindEntityType(typeof(TenantHealthStatus));
            var schema = entityType.GetSchema();
            _store.ProductTenantHealthStatusTableName = entityType.GetTableName();



            _store.SetHealthCheckSettings((await _settingService.LoadSettingAsync<HealthCheckSettings>()).Data);
        }
    }
}

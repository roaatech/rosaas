using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Tenants.BackgroundServices;
using Roaa.Rosas.Application.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.HealthCheckStatus.Services
{
    public class TenantHealthCheckService : ITenantHealthCheckService
    {
        #region Props
        private readonly ILogger<TenantHealthCheckService> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public TenantHealthCheckService(ILogger<TenantHealthCheckService> logger,
                                        BackgroundServicesStore backgroundWorkerStore,
                                        IExternalSystemAPI externalSystemAPI,
                                        IRosasDbContext dbContext)
        {
            _logger = logger;
            _externalSystemAPI = externalSystemAPI;
            _dbContext = dbContext;
            _backgroundWorkerStore = backgroundWorkerStore;
        }
        #endregion


        #region Services     
        public async Task AddTenantHealthCheckStatusAsync(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var entity = new TenantHealthCheck
            {
                Id = Guid.NewGuid(),
                TimeStamp = date,
                Created = date,
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                IsHealthy = isAvailable,
                Duration = (int)duration,
                HealthCheckUrl = healthCheckUrl
            };

            _dbContext.TenantHealthChecks.Add(entity);

            await _dbContext.SaveChangesAsync();





            Type dss = GetType();
            string checkDateUpdatingQuery = "";
            ProductTenantHealthStatus hs = null;
            List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(hs.HealthCheckUrl)}", healthCheckUrl),
                                new MySqlParameter($"@{nameof(hs.IsHealthy)}", isAvailable),
                                new MySqlParameter($"@{nameof(hs.Duration)}", duration),
                                new MySqlParameter($"@{nameof(hs.LastCheckDate)}",  date),
                                new MySqlParameter($"@{nameof(hs.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(hs.ProductId)}", jobTask.ProductId),
                            };

            if (GetType() == typeof(AvailableTenantChecker) && !isAvailable)
            {
                paramItems.Add(new MySqlParameter($"@{nameof(hs.CheckDate)}", date));

                checkDateUpdatingQuery = $"{nameof(hs.CheckDate)} = @{nameof(hs.CheckDate)} ,";
            }

            var commandText = @$"UPDATE {_backgroundWorkerStore.ProductTenantHealthStatusTableName} 
                                        SET     {checkDateUpdatingQuery}
                                                {nameof(hs.HealthCheckUrl)} = @{nameof(hs.HealthCheckUrl)} , 
                                                {nameof(hs.IsHealthy)} = @{nameof(hs.IsHealthy)} ,  
                                                {nameof(hs.Duration)} = @{nameof(hs.Duration)} ,  
                                                {nameof(hs.LastCheckDate)} = @{nameof(hs.LastCheckDate)}
                                        WHERE   {nameof(hs.TenantId)} =  @{nameof(hs.TenantId)}  
                                        AND     {nameof(hs.ProductId)} =  @{nameof(hs.ProductId)}
                                 ";

            var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, cancellationToken);
        }


        public async Task AddExternalSystemDispatchAsync(JobTask jobTask, double duration, string url, bool isSuccessful, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var entity = new ExternalSystemDispatch
            {
                Id = Guid.NewGuid(),
                TimeStamp = date,
                DispatchDate = date,
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                Duration = (int)duration,
                IsSuccessful = isSuccessful,
                Url = url
            };

            _dbContext.ExternalSystemDispatches.Add(entity);

            await _dbContext.SaveChangesAsync();





        }
        public TenantHealthCheck AddTenantAvailabilityToDbContext(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable)
        {
            var date = DateTime.UtcNow;
            var entity = new TenantHealthCheck
            {
                Id = Guid.NewGuid(),
                TimeStamp = date,
                Created = date,
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                IsHealthy = isAvailable,
                Duration = (int)duration,
                HealthCheckUrl = healthCheckUrl
            };

            _dbContext.TenantHealthChecks.Add(entity);

            return entity;
        }

        public JobTask AddJobTaskToDbContext(JobTask jobTask, JobTaskType type)
        {
            var entity = new JobTask
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                Type = type,
            };

            _dbContext.JobTasks.Add(entity);

            return entity;
        }

        public async Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(JobTask jobTask, string tenantHhealthCheckUrl, CancellationToken cancellationToken)
        {
            var requestResult = await _externalSystemAPI.CheckTenantHealthStatusAsync(new ExternalSystemRequestModel<CheckTenantAvailabilityModel>
            {
                BaseUrl = tenantHhealthCheckUrl,
                Data = new()
                {
                    TenantId = jobTask.TenantId,
                }
            }, cancellationToken);

            return requestResult;
        }

        public async Task<string> GetTenantHhealthCheckUrlAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            return await _dbContext.ProductTenants
                                               .Where(x => x.ProductId == jobTask.ProductId && x.TenantId == jobTask.TenantId)
                                               .Select(x => x.HealthCheckUrl)
                                               .SingleOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        public async Task AddInaccessibleJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var inaccessibleJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Inaccessible);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _backgroundWorkerStore.AddInaccessibleTenantTask(inaccessibleJobTask);
        }

        public async Task AddUnavailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var unavailableJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Unavailable);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _backgroundWorkerStore.AddUnavailableTenantTask(unavailableJobTask);
        }

        public async Task AddInformerJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var informerJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Informer);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _backgroundWorkerStore.AddInformerTask(informerJobTask);
        }

        public async Task RemoveAvailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            _backgroundWorkerStore.RemoveJobTask(jobTask);
        }

        public async Task RemoveUnavailableJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            await RemoveJobTaskAsync(jobTask, cancellationToken);

            _backgroundWorkerStore.RemoveUnavailableTenantTask(jobTask);
        }

        public async Task RemoveInaccessibleJobTaskTasks(JobTask jobTask, CancellationToken cancellationToken)
        {
            await RemoveJobTaskAsync(jobTask, cancellationToken);

            _backgroundWorkerStore.RemoveInaccessibleTenantsTasks(jobTask);
        }

        public async Task RemoveJobTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            _dbContext.Entry(jobTask).State = EntityState.Deleted;

            _dbContext.JobTasks.Remove(jobTask);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> InformExternalSystemTheTenantIsUnavailableAsync(JobTask jobTask, string healthCheckStatusUrl, CancellationToken cancellationToken)
        {
            var requestResult = await _externalSystemAPI.InformTheTenantUnavailabilityAsync(new ExternalSystemRequestModel<InformTenantAvailabilityModel>
            {
                BaseUrl = healthCheckStatusUrl,
                Data = new()
                {
                    TenantId = jobTask.TenantId,
                }
            }, cancellationToken);

            return requestResult;
        }

        public async Task<string> GetHealthCheckStatusUrlOfExternalSystemAsync(JobTask jobTask, IProductService productService, CancellationToken cancellationToken)
        {
            Expression<Func<Product, string>> selector = x => x.HealthStatusChangeUrl;

            var urlItemResult = await productService.GetProductEndpointByIdAsync(jobTask.ProductId, selector, cancellationToken);

            return urlItemResult.Data;
        }


        #endregion

    }
}


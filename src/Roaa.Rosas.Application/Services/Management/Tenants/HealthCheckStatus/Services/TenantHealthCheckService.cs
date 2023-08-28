using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Services
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
        public async Task AddTenantHealthCheckStatusAsync(Type backgroundServiceType, JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var entity = new TenantHealthCheckHistory
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

            _dbContext.TenantHealthCheckHistory.Add(entity);

            await _dbContext.SaveChangesAsync();





            Type dss = GetType();
            string partOfQuery = "";
            TenantHealthStatus hs = null;
            List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(hs.HealthCheckUrl)}", healthCheckUrl),
                                new MySqlParameter($"@{nameof(hs.IsHealthy)}", isAvailable),
                                new MySqlParameter($"@{nameof(hs.Duration)}", duration),
                                new MySqlParameter($"@{nameof(hs.LastCheckDate)}",  date),
                                new MySqlParameter($"@{nameof(hs.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(hs.ProductId)}", jobTask.ProductId),
                            };

            if (isAvailable)
            {
                partOfQuery += $" {nameof(hs.HealthyCount)} = {nameof(hs.HealthyCount)} + 1 ,";
            }
            else
            {
                partOfQuery += $" {nameof(hs.UnhealthyCount)} = {nameof(hs.UnhealthyCount)} + 1 ,";
            }


            if (backgroundServiceType == typeof(AvailableTenantChecker) && !isAvailable)
            {
                paramItems.Add(new MySqlParameter($"@{nameof(hs.CheckDate)}", date));

                partOfQuery += $" {nameof(hs.CheckDate)} = @{nameof(hs.CheckDate)} ,";
            }

            var commandText = @$"UPDATE {_backgroundWorkerStore.TenantHealthStatusTableName} 
                                        SET     {partOfQuery}
                                                {nameof(hs.HealthCheckUrl)} = @{nameof(hs.HealthCheckUrl)} , 
                                                {nameof(hs.IsHealthy)} = @{nameof(hs.IsHealthy)} ,  
                                                {nameof(hs.Duration)} = @{nameof(hs.Duration)} ,  
                                                {nameof(hs.LastCheckDate)} = @{nameof(hs.LastCheckDate)}
                                        WHERE   {nameof(hs.TenantId)} =  @{nameof(hs.TenantId)}  
                                        AND     {nameof(hs.ProductId)} =  @{nameof(hs.ProductId)}
                                 ";

            var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, cancellationToken);
        }

        public async Task AddTenantProcessHistoryAsHealthyStatusAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            await AddTenantProcessHistoryAsync(jobTask, TenantProcessType.HealthyStatus, true, cancellationToken);
        }

        public async Task AddTenantProcessHistoryAsUnhealthyStatusAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            await AddTenantProcessHistoryAsync(jobTask, TenantProcessType.UnhealthStatus, true, cancellationToken);
        }

        private async Task<Guid> AddTenantProcessHistoryAsync(JobTask jobTask, TenantProcessType processType, bool enabled, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var status = await _dbContext.ProductTenants
                                      .Where(x => x.TenantId == jobTask.TenantId &&
                                                  x.ProductId == jobTask.ProductId)
                                      .Select(x => x.Status)
                                      .SingleOrDefaultAsync(cancellationToken);

            var processHistory = new TenantProcessHistory
            {
                Id = Guid.NewGuid(),
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                OwnerType = UserType.RosasSystem,
                Status = status,
                ProcessDate = date,
                TimeStamp = date,
                ProcessType = processType,
                Enabled = enabled,
            };

            _dbContext.TenantProcessHistory.Add(processHistory);

            await _dbContext.SaveChangesAsync();

            _backgroundWorkerStore.AddTenantProcess(jobTask.TenantId, jobTask.ProductId, processHistory.Id);

            return processHistory.Id;
        }

        public async Task AddTenantProcessHistoryAsExternalSystemInformedAsync(JobTask jobTask, bool success, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;

            var processHistory = new TenantProcessHistory
            {
                Id = Guid.NewGuid(),
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                OwnerType = UserType.RosasSystem,
                ProcessDate = date,
                TimeStamp = date,
                ProcessType = success ? TenantProcessType.ExternalSystemSuccessfullyInformed : TenantProcessType.FailedToInformExternalSystem,
                Enabled = true,
            };

            _dbContext.TenantProcessHistory.Add(processHistory);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTenantProcessHistoryAsHealthCheckStatusAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var processId = _backgroundWorkerStore.GetTenantProcess(jobTask.TenantId, jobTask.ProductId);
            if (processId is null)
            {
                if (jobTask.Type == JobTaskType.Unavailable)
                {
                    processId = await AddTenantProcessHistoryAsync(jobTask, TenantProcessType.UnhealthStatus, true, cancellationToken);
                }
                else
                {
                    processId = await AddTenantProcessHistoryAsync(jobTask, TenantProcessType.HealthyStatus, true, cancellationToken);
                }
            }
            TenantProcessHistory entity = null;
            List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(entity.Enabled)}", true),
                                new MySqlParameter($"@{nameof(entity.ProcessDate)}", date),
                                new MySqlParameter($"@{nameof(entity.TimeStamp)}", date.Ticks),
                                new MySqlParameter($"@{nameof(entity.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(entity.ProductId)}", jobTask.ProductId),
                                new MySqlParameter($"@{nameof(entity.Id)}", processId),
                            };

            var commandText = @$"UPDATE {_backgroundWorkerStore.TenantProcessHistoryTableName} 
                                        SET 
                                                {nameof(entity.Enabled)} = @{nameof(entity.Enabled)}, 
                                                {nameof(entity.ProcessDate)} = @{nameof(entity.ProcessDate)} , 
                                                {nameof(entity.TimeStamp)} = @{nameof(entity.TimeStamp)}  
                                        WHERE   {nameof(entity.TenantId)} =  @{nameof(entity.TenantId)}  
                                        AND     {nameof(entity.ProductId)} =  @{nameof(entity.ProductId)}
                                        AND     {nameof(entity.Id)} =  @{nameof(entity.Id)}
                                 ";

            var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, cancellationToken);
        }

        public async Task ResetTenantHealthStatusCountersAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            int count = 0;
            TenantHealthStatus hs = null;
            List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(hs.HealthyCount)}", count),
                                new MySqlParameter($"@{nameof(hs.UnhealthyCount)}", count),
                                new MySqlParameter($"@{nameof(hs.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(hs.ProductId)}", jobTask.ProductId),
                            };

            var commandText = @$"UPDATE {_backgroundWorkerStore.TenantHealthStatusTableName} 
                                      SET 
                                                {nameof(hs.HealthyCount)} = @{nameof(hs.HealthyCount)} , 
                                                {nameof(hs.UnhealthyCount)} = @{nameof(hs.UnhealthyCount)}  
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

        public TenantHealthCheckHistory AddTenantHealthCheckHistoryToDbContext(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable)
        {
            var date = DateTime.UtcNow;
            var entity = new TenantHealthCheckHistory
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

            _dbContext.TenantHealthCheckHistory.Add(entity);

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
            string tenantName = _backgroundWorkerStore.GetTenantName(jobTask.TenantId);


            var requestResult = await _externalSystemAPI.CheckTenantHealthStatusAsync(new ExternalSystemRequestModel<CheckTenantHealthStatusModel>
            {
                BaseUrl = tenantHhealthCheckUrl,
                TenantId = jobTask.TenantId,
                Data = new()
                {
                    TenantName = tenantName,
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
        public async Task AddAvailableTenantTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            _backgroundWorkerStore.AddAvailableTenantTask(new JobTask
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                TenantId = jobTask.TenantId,
                ProductId = jobTask.ProductId,
                Type = JobTaskType.Available,
            }, "");
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


        public async Task<Result<ExternalSystemResultModel<dynamic>>> InformExternalSystemTheTenantIsUnavailableAsync(JobTask jobTask, ProductApiModel productApi, CancellationToken cancellationToken)
        {
            var tenantName = await GetTenantNameAsync(jobTask, cancellationToken);

            var requestResult = await _externalSystemAPI.InformTheTenantUnavailableAsync(new ExternalSystemRequestModel<InformTheTenantUnavailableModel>
            {
                BaseUrl = productApi.Url,
                ApiKey = productApi.ApiKey,
                TenantId = jobTask.TenantId,
                Data = new()
                {
                    TenantName = tenantName,
                }
            }, cancellationToken);

            return requestResult;
        }

        public async Task<string> GetHealthCheckStatusUrlOfExternalSystemAsync(JobTask jobTask, IProductService productService, CancellationToken cancellationToken)
        {
            Expression<Func<Product, string>> selector = x => x.HealthStatusInformerUrl;

            var urlItemResult = await productService.GetProductEndpointByIdAsync(jobTask.ProductId, selector, cancellationToken);

            return urlItemResult.Data;
        }


        public async Task<string> GetTenantNameAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var tenantName = await _dbContext.Tenants
                                       .Where(x => x.Id == jobTask.TenantId)
                                       .Select(x => x.UniqueName)
                                       .SingleOrDefaultAsync(cancellationToken);
            return tenantName;
        }


        #endregion

    }
}


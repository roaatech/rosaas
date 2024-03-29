﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Services
{
    public class TenantHealthCheckService : ITenantHealthCheckService
    {
        #region Props
        private readonly ILogger<TenantHealthCheckService> _logger;
        private readonly BackgroundServicesStore _backgroundWorkerStore;
        private readonly IExternalSystemAPI _externalSystemAPI;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public TenantHealthCheckService(ILogger<TenantHealthCheckService> logger,
                                        BackgroundServicesStore backgroundWorkerStore,
                                        IExternalSystemAPI externalSystemAPI,
                                        IRosasDbContext dbContext,
                                        IPublisher publisher)
        {
            _logger = logger;
            _externalSystemAPI = externalSystemAPI;
            _dbContext = dbContext;
            _backgroundWorkerStore = backgroundWorkerStore;
            _publisher = publisher;
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
                CreationDate = date,
                SubscriptionId = jobTask.SubscriptionId,
                ProductId = jobTask.ProductId,
                TenantId = jobTask.TenantId,
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
                                new MySqlParameter($"@{nameof(hs.IsChecked)}", true),
                                new MySqlParameter($"@{nameof(hs.Duration)}", duration),
                                new MySqlParameter($"@{nameof(hs.LastCheckDate)}",  date),
                                new MySqlParameter($"@{nameof(hs.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(hs.ProductId)}", jobTask.ProductId),
                                new MySqlParameter($"@{nameof(hs.SubscriptionId)}", jobTask.SubscriptionId),
                            };

            if (isAvailable)
            {
                partOfQuery += $" {nameof(hs.HealthyCount)} = {nameof(hs.HealthyCount)} + 1 ,";
            }
            else
            {
                partOfQuery += $" {nameof(hs.UnhealthyCount)} = {nameof(hs.UnhealthyCount)} + 1 ,";
            }


            if (backgroundServiceType == typeof(AvailableTenantHealthCheckWorker) && !isAvailable)
            {
                paramItems.Add(new MySqlParameter($"@{nameof(hs.CheckDate)}", date));

                partOfQuery += $" {nameof(hs.CheckDate)} = @{nameof(hs.CheckDate)} ,";
            }

            var commandText = @$"UPDATE {_backgroundWorkerStore.TenantHealthStatusTableName} 
                                        SET     {partOfQuery}
                                                {nameof(hs.HealthCheckUrl)} = @{nameof(hs.HealthCheckUrl)} , 
                                                {nameof(hs.IsHealthy)} = @{nameof(hs.IsHealthy)} ,  
                                                {nameof(hs.IsChecked)} = @{nameof(hs.IsChecked)} ,  
                                                {nameof(hs.Duration)} = @{nameof(hs.Duration)} ,  
                                                {nameof(hs.LastCheckDate)} = @{nameof(hs.LastCheckDate)}
                                        WHERE   {nameof(hs.TenantId)} =  @{nameof(hs.TenantId)}  
                                        AND     {nameof(hs.ProductId)} =  @{nameof(hs.ProductId)}
                                        AND     {nameof(hs.SubscriptionId)} =  @{nameof(hs.SubscriptionId)}
                                 ";

            var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, cancellationToken);
        }



        public async Task<Guid> PublishTenantProcessingCompletedEventAsync(JobTask jobTask, TenantProcessType processType, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions
                                      .Where(x => x.TenantId == jobTask.TenantId &&
                                                  x.ProductId == jobTask.ProductId &&
                                                  x.Id == jobTask.SubscriptionId)
                                      .SingleOrDefaultAsync(cancellationToken);

            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                        processType,
                                        true,
                                        null,
                                        out Guid processId,
                                        subscription));

            _backgroundWorkerStore.AddTenantProcess(jobTask.TenantId, jobTask.ProductId, processId);

            return processId;
        }

        public async Task PublishTenantProcessingCompletedEventAsExternalSystemInformedAsync(JobTask jobTask, bool success, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions
                                      .Where(x => x.TenantId == jobTask.TenantId &&
                                                  x.ProductId == jobTask.ProductId &&
                                                  x.Id == jobTask.SubscriptionId)
                                      .SingleOrDefaultAsync(cancellationToken);

            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                       success ? TenantProcessType.ExternalSystemSuccessfullyInformed : TenantProcessType.FailedToInformExternalSystem,
                                        true,
                                        null,
                                        out _,
                                        subscription));
        }

        public async Task UpdateTenantProcessHistoryAsHealthCheckStatusAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            var date = DateTime.UtcNow;
            var processId = _backgroundWorkerStore.GetTenantProcess(jobTask.TenantId, jobTask.ProductId);
            if (processId is null)
            {
                if (jobTask.Type == JobTaskType.Unavailable)
                {
                    processId = await PublishTenantProcessingCompletedEventAsync(jobTask, TenantProcessType.UnhealthyStatus, cancellationToken);
                }
                else
                {
                    processId = await PublishTenantProcessingCompletedEventAsync(jobTask, TenantProcessType.HealthyStatus, cancellationToken);
                }
            }
            else
            {
                TenantProcessHistory entity = null;
                List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(entity.Enabled)}", true),
                                new MySqlParameter($"@{nameof(entity.ProcessDate)}", date),
                                new MySqlParameter($"@{nameof(entity.TimeStamp)}", date.Ticks),
                                new MySqlParameter($"@{nameof(entity.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(entity.ProductId)}", jobTask.ProductId),
                                new MySqlParameter($"@{nameof(entity.SubscriptionId)}", jobTask.SubscriptionId),
                                new MySqlParameter($"@{nameof(entity.Id)}", processId),
                            };

                var commandText = @$"UPDATE {_backgroundWorkerStore.TenantProcessHistoryTableName} 
                                        SET  
                                                {nameof(entity.UpdatesCount)} = {nameof(entity.UpdatesCount)} + 1 ,
                                                {nameof(entity.Enabled)} = @{nameof(entity.Enabled)}, 
                                                {nameof(entity.ProcessDate)} = @{nameof(entity.ProcessDate)} , 
                                                {nameof(entity.TimeStamp)} = @{nameof(entity.TimeStamp)}  
                                        WHERE   {nameof(entity.TenantId)} =  @{nameof(entity.TenantId)}  
                                        AND     {nameof(entity.ProductId)} =  @{nameof(entity.ProductId)}
                                        AND     {nameof(entity.SubscriptionId)} =  @{nameof(entity.SubscriptionId)}
                                        AND     {nameof(entity.Id)} =  @{nameof(entity.Id)}
                                 ";

                var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, cancellationToken);
            }
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
                                new MySqlParameter($"@{nameof(hs.SubscriptionId)}", jobTask.SubscriptionId),
                            };

            var commandText = @$"UPDATE {_backgroundWorkerStore.TenantHealthStatusTableName} 
                                      SET 
                                                {nameof(hs.HealthyCount)} = @{nameof(hs.HealthyCount)} , 
                                                {nameof(hs.UnhealthyCount)} = @{nameof(hs.UnhealthyCount)}  
                                        WHERE   {nameof(hs.TenantId)} =  @{nameof(hs.TenantId)}  
                                        AND     {nameof(hs.ProductId)} =  @{nameof(hs.ProductId)}
                                        AND     {nameof(hs.SubscriptionId)} =  @{nameof(hs.SubscriptionId)}
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
                SubscriptionId = jobTask.SubscriptionId,
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
                CreationDate = date,
                SubscriptionId = jobTask.SubscriptionId,
                ProductId = jobTask.ProductId,
                TenantId = jobTask.TenantId,
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
                CreationDate = DateTime.UtcNow,
                SubscriptionId = jobTask.SubscriptionId,
                ProductId = jobTask.ProductId,
                TenantId = jobTask.TenantId,
                Type = type,
            };

            _dbContext.JobTasks.Add(entity);

            return entity;
        }

        public async Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(JobTask jobTask, string tenantHhealthCheckUrl, CancellationToken cancellationToken)
        {
            string tenantName = _backgroundWorkerStore.GetTenantName(jobTask.TenantId);
            string productsAPIKey = _backgroundWorkerStore.GetProductAPIKey(jobTask.ProductId);


            var requestResult = await _externalSystemAPI.CheckTenantHealthStatusAsync(new ExternalSystemRequestModel<CheckTenantHealthStatusModel>
            {
                BaseUrl = tenantHhealthCheckUrl,
                ApiKey = productsAPIKey,
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
            return await _dbContext.Subscriptions
                                               .Where(x => x.Id == jobTask.SubscriptionId &&
                                                           x.ProductId == jobTask.ProductId &&
                                                           x.TenantId == jobTask.TenantId)
                                               .Select(x => x.HealthCheckUrl)
                                               .SingleOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        public async Task AddAvailableTenantTaskAsync(JobTask jobTask, CancellationToken cancellationToken)
        {
            _backgroundWorkerStore.AddAvailableTenantTask(new JobTask
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                SubscriptionId = jobTask.SubscriptionId,
                ProductId = jobTask.ProductId,
                TenantId = jobTask.TenantId,
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
                                       .Select(x => x.SystemName)
                                       .SingleOrDefaultAsync(cancellationToken);
            return tenantName;
        }


        #endregion

    }
}


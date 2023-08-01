using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using System.Collections.Concurrent;
using System.Linq.Expressions;




namespace Roaa.Rosas.Application.Tenants.BackgroundServices.Workers.abstruct
{
    public abstract class BaseWorker : BackgroundService
    {
        protected readonly ILogger<BackgroundServiceManager> _logger;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly BackgroundWorkerStore _backgroundWorkerStore;
        protected readonly BlockingCollection<JobTask> _tasks;
        protected IExternalSystemAPI _externalSystemAPI;
        protected IRosasDbContext _dbContext;
        protected long cycleIndex = 1;
        protected long taskIndex = 1;
        protected abstract TimeSpan _period { get; set; }



        public BaseWorker(ILogger<BackgroundServiceManager> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  BackgroundWorkerStore backgroundWorkerStore)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _backgroundWorkerStore = backgroundWorkerStore;
            _tasks = new BlockingCollection<JobTask>();
            _dbContext = null;
        }



        protected async Task AddTenantAvailabilityHistoryAsync(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable, CancellationToken stoppingToken)
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





            Type dss = this.GetType();
            string checkDateUpdatingQuery = "";
            ProductTenantHealthStatus hs = null;
            List<object> paramItems = new List<object>
                            {
                                new MySqlParameter($"@{nameof(hs.HealthCheckUrl)}", healthCheckUrl),
                                new MySqlParameter($"@{nameof(hs.IsHealthy)}", isAvailable),
                                new MySqlParameter($"@{nameof(hs.LastCheckDate)}",  date),
                                new MySqlParameter($"@{nameof(hs.TenantId)}", jobTask.TenantId),
                                new MySqlParameter($"@{nameof(hs.ProductId)}", jobTask.ProductId),
                            };

            if (this.GetType() == typeof(AvailableTenantChecker) && !isAvailable)
            {
                paramItems.Add(new MySqlParameter($"@{nameof(hs.CheckDate)}", date));

                checkDateUpdatingQuery = $"{nameof(hs.CheckDate)} = @{nameof(hs.CheckDate)} ,";
            }

            var commandText = @$"UPDATE {_backgroundWorkerStore.ProductTenantHealthStatusTableName} 
                                        SET     {checkDateUpdatingQuery}
                                                {nameof(hs.HealthCheckUrl)} = @{nameof(hs.HealthCheckUrl)} , 
                                                {nameof(hs.IsHealthy)} = @{nameof(hs.IsHealthy)} ,  
                                                {nameof(hs.LastCheckDate)} = @{nameof(hs.LastCheckDate)}
                                        WHERE   {nameof(hs.TenantId)} =  @{nameof(hs.TenantId)}  
                                        AND     {nameof(hs.ProductId)} =  @{nameof(hs.ProductId)}
                                 ";

            var res = await _dbContext.Database.ExecuteSqlRawAsync(commandText, paramItems, stoppingToken);
        }

        protected TenantHealthCheck AddTenantAvailabilityToDbContext(JobTask jobTask, double duration, string healthCheckUrl, bool isAvailable)
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

        protected JobTask AddJobTaskToDbContext(JobTask jobTask, JobTaskType type)
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


        protected async Task<bool> CheckTenantHealthStatusAndRecordResultAsync(JobTask jobTask, CancellationToken stoppingToken)
        {

            var healthCheckUrl = await _dbContext.ProductTenants
                                               .Where(x => x.ProductId == jobTask.ProductId && x.TenantId == jobTask.TenantId)
                                               .Select(x => x.HealthCheckUrl)
                                               .SingleOrDefaultAsync(stoppingToken);


            Log($"##checks the tenant's health/heartbeat, [TenantId:{{0}}], [ProductId:{{1}}], [Url:{{2}}]",
                jobTask.TenantId,
                jobTask.ProductId,
                healthCheckUrl);

            var requestResult = await _externalSystemAPI.CheckTenantHealthStatusAsync(new ExternalSystemRequestModel<CheckTenantAvailabilityModel>
            {
                BaseUrl = healthCheckUrl,
                Data = new()
                {
                    TenantId = jobTask.TenantId,
                }
            }, stoppingToken);

            Log($"##founds the tenan is {{0}} [TenantId:{{1}}], [ProductId:{{2}}]",
               requestResult.Success ? "Available" : "Unavailable",
               jobTask.TenantId,
               jobTask.ProductId);



            await AddTenantAvailabilityHistoryAsync(jobTask, 0, healthCheckUrl, requestResult.Success, stoppingToken);

            return requestResult.Success;
        }



        protected async Task AddInaccessibleJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            var inaccessibleJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Inaccessible);

            await _dbContext.SaveChangesAsync(stoppingToken);

            _backgroundWorkerStore.AddInaccessibleTenantTask(inaccessibleJobTask);
        }

        protected async Task AddUnavailableJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            var unavailableJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Unavailable);

            await _dbContext.SaveChangesAsync(stoppingToken);

            _backgroundWorkerStore.AddUnavailableTenantTask(unavailableJobTask);
        }

        protected async Task AddInformerJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            var informerJobTask = AddJobTaskToDbContext(jobTask, JobTaskType.Informer);

            await _dbContext.SaveChangesAsync(stoppingToken);

            _backgroundWorkerStore.AddInformerTask(informerJobTask);
        }

        protected async Task RemoveAvailableJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            _backgroundWorkerStore.RemoveJobTask(jobTask);
        }

        protected async Task RemoveUnavailableJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            await RemoveJobTaskAsync(jobTask, stoppingToken);

            _backgroundWorkerStore.RemoveUnavailableTenantTask(jobTask);
        }

        protected async Task RemoveJobTaskAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            //var jobToRemove = await _dbContext.JobTasks
            //                                    .Where(x => x.Id == jobTask.Id)
            //                                    .SingleOrDefaultAsync(stoppingToken);

            _dbContext.Entry(jobTask).State = EntityState.Deleted;

            _dbContext.JobTasks.Remove(jobTask);

            await _dbContext.SaveChangesAsync(stoppingToken);
        }



        protected async Task<bool> InformExternalSystemTheTenantIsUnavailableAsync(JobTask jobTask, IProductService productService, CancellationToken stoppingToken)
        {
            Expression<Func<Product, string>> selector = x => x.HealthStatusChangeUrl;

            var urlItemResult = await productService.GetProductEndpointByIdAsync(jobTask.ProductId, selector, stoppingToken);

            Log($"##informs the external system that the is {{0}}, [TenantId:{{1}}], [ProductId:{{2}}], [Url:{{3}}]",
                    "Unavailable(Down)",
                jobTask.TenantId,
                jobTask.ProductId,
                urlItemResult.Data);



            var requestResult = await _externalSystemAPI.InformTheTenantUnavailabilityAsync(new ExternalSystemRequestModel<InformTenantAvailabilityModel>
            {
                BaseUrl = urlItemResult.Data,
                Data = new()
                {
                    TenantId = jobTask.TenantId,
                }
            }, stoppingToken);


            Log($"##received a {{0}} response from the external system [ProductId:{{1}}]",
                requestResult.Success ? "successful" : "failed",
                jobTask.ProductId);


            return requestResult.Success;
        }


        protected void Log(string? message, params object?[] args)
        {

            var fullArgsLength = args.Length + 2 + (message.StartsWith("##") ? 1 : 0) + (message.StartsWith("#") ? 1 : 0);

            object?[] fullArgs = new object[fullArgsLength];

            fullArgs[0] = GetType().Name;

            for (int i = 0; i < args.Length; i++)
            {
                var displacement = fullArgsLength - (args.Length + 1);
                var argIndex = $"{{{i}}}";
                message = message.Replace(argIndex, $"{{{i + displacement}}}");
                fullArgs[i + displacement] = args[i];
            }

            if (message.StartsWith("##"))
            {
                fullArgs[1] = cycleIndex;
                fullArgs[2] = taskIndex;
                message = message.Replace("##", "[#{1}]-[##{2}] ");
            }

            if (message.StartsWith("#"))
            {
                fullArgs[1] = cycleIndex;
                message = message.Replace("#", "[#{1}] ");
            }

            fullArgs[fullArgsLength - 1] = DateTime.UtcNow.ToString("O");

            var dateTimeIndex = $"{{{fullArgsLength - 1}}}";

            _logger.LogInformation($"{{{0}}} Background Service {message}. [{dateTimeIndex}]", fullArgs);
        }


    }
}
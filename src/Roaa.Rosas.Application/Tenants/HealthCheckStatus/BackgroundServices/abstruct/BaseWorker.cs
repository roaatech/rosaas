using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Tenants.BackgroundServices;
using Roaa.Rosas.Application.Tenants.HealthCheckStatus.Services;
using Roaa.Rosas.Domain.Entities.Management;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.HealthCheckStatus.BackgroundServices.abstruct
{

    public abstract class BaseWorker : BackgroundService
    {
        protected readonly ILogger<BaseWorker> _logger;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly BackgroundServicesStore _backgroundWorkerStore;
        protected readonly BlockingCollection<JobTask> _tasks;
        protected ITenantHealthCheckService _tenantHealthCheckService;
        protected long cycleIndex = 1;
        protected long taskIndex = 1;
        protected Guid tenantId = Guid.Empty;
        protected Guid productId = Guid.Empty;
        protected abstract TimeSpan _period { get; set; }



        public BaseWorker(ILogger<BaseWorker> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  BackgroundServicesStore backgroundWorkerStore)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _backgroundWorkerStore = backgroundWorkerStore;
            _tasks = new BlockingCollection<JobTask>();
            _tenantHealthCheckService = null;
        }




        protected bool IsPeriodUpdated(int periodInSeconds)
        {
            return _period.TotalSeconds != periodInSeconds;
        }

        protected int ToSeconds(int periodInMinutes)
        {
            return periodInMinutes * 60;
        }
        protected async Task<bool> CheckTenantHealthStatusAndRecordResultAsync(JobTask jobTask, CancellationToken stoppingToken)
        {
            var healthCheckUrl = await _tenantHealthCheckService.GetTenantHhealthCheckUrlAsync(jobTask, stoppingToken);


            Log($"##checks the tenant's health/heartbeat, [TenantId:{{0}}], [ProductId:{{1}}], [Url:{{2}}]",
                jobTask.TenantId,
                jobTask.ProductId,
                healthCheckUrl);

            var requestResult = await _tenantHealthCheckService.CheckTenantHealthStatusAsync(jobTask, healthCheckUrl, stoppingToken);

            Log($"##founds the tenant is {{0}} [TenantId:{{1}}], [ProductId:{{2}}]",
               requestResult.Success ? "Available" : "Unavailable",
               jobTask.TenantId,
               jobTask.ProductId);

            await _tenantHealthCheckService.AddTenantHealthCheckStatusAsync(GetType(), jobTask, requestResult.Data.DurationInMillisecond, requestResult.Data.Url, requestResult.Success, stoppingToken);

            return requestResult.Success;
        }






        protected async Task<bool> InformExternalSystemTheTenantIsUnavailableAsync(JobTask jobTask, IProductService productService, CancellationToken stoppingToken)
        {
            Expression<Func<Product, string>> selector = x => x.HealthStatusChangeUrl;

            var urlItemResult = await productService.GetProductEndpointByIdAsync(jobTask.ProductId, selector, stoppingToken);

            string text = "Unavailable(Down)";
            Log($"##informs the external system that the is {{0}}, [TenantId:{{1}}], [ProductId:{{2}}], [Url:{{3}}]",
                text,
                jobTask.TenantId,
                jobTask.ProductId,
                urlItemResult.Data);


            var requestResult = await _tenantHealthCheckService.InformExternalSystemTheTenantIsUnavailableAsync(jobTask, urlItemResult.Data, stoppingToken);


            Log($"##received a {{0}} response from the external system [ProductId:{{1}}]",
                requestResult.Success ? "successful" : "failed",
                jobTask.ProductId);

            await _tenantHealthCheckService.AddExternalSystemDispatchAsync(jobTask, requestResult.Data.DurationInMillisecond, requestResult.Data.Url, requestResult.Success, stoppingToken);

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
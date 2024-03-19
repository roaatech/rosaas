using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Services;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices.abstruct;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices
{
    public class InformerHealthCheckWorker : BaseWorker
    {
        protected override TimeSpan _period { get; set; } = TimeSpan.FromSeconds(60 * 1);

        public InformerHealthCheckWorker(ILogger<InformerHealthCheckWorker> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  BackgroundServicesStore backgroundWorkerStore)
       : base(logger, serviceScopeFactory, backgroundWorkerStore)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log("Started. It's will execute its work every [{0}] seconds", _period.TotalSeconds);

            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!cancellationToken.IsCancellationRequested &&
                 await timer.WaitForNextTickAsync(cancellationToken))
            {
                Log($"#Cycle Execution Started");

                await CheckAsync(cancellationToken);

                Log($"#Cycle Execution Finished");
                cycleIndex++;
            }
        }

        private async Task CheckAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _tenantHealthCheckService = scope.ServiceProvider.GetRequiredService<ITenantHealthCheckService>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            while (!_backgroundWorkerStore.InformerTasks.IsCompleted)
            {
                try
                {
                    Log($"#Try to take a job Task");

                    if (_backgroundWorkerStore.InformerTasks.TryTake(out var jobTask) &&
                            _backgroundWorkerStore.MakeSureIsNotRemoved(jobTask))
                    {
                        Log($"##Took the JobTask, for the tenant: [TenantId:{{0}}], [ProductId:{{1}}]", jobTask.TenantId, jobTask.ProductId);

                        tenantId = jobTask.TenantId;
                        productId = jobTask.ProductId;

                        var success = await InformExternalSystemTheTenantIsUnavailableAsync(jobTask, productService, cancellationToken);

                        if (success)
                        {
                            await _tenantHealthCheckService.PublishTenantProcessingCompletedEventAsExternalSystemInformedAsync(jobTask, success, cancellationToken);
                            await _tenantHealthCheckService.RemoveJobTaskAsync(jobTask, cancellationToken);
                        }
                        else
                        {
                            await _tenantHealthCheckService.PublishTenantProcessingCompletedEventAsExternalSystemInformedAsync(jobTask, success, cancellationToken);
                        }
                    }
                    else
                    {
                        Log($"#not founds a job Task");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred on {0} while processing the Job Task related to the tenant [TenantId:{1}], [ProductId:{2}]", GetType().Name, tenantId, productId);
                }
                finally
                {

                }
                taskIndex++;
            }
        }
    }
}

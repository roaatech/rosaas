using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Tenants.BackgroundServices.Workers.abstruct;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices.Workers
{
    public class Informer : BaseWorker
    {
        protected override TimeSpan _period { get; set; } = TimeSpan.FromSeconds(60 * 1);

        public Informer(ILogger<BackgroundServiceManager> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  BackgroundWorkerStore backgroundWorkerStore)
       : base(logger, serviceScopeFactory, backgroundWorkerStore)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log("Started. It's will execute its work every [{0}] seconds", _period.TotalSeconds);

            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested &&
                 await timer.WaitForNextTickAsync(stoppingToken))
            {
                Log($"#Cycle Execution Started");

                using var scope = _serviceScopeFactory.CreateScope();
                _dbContext = scope.ServiceProvider.GetRequiredService<IRosasDbContext>();
                _externalSystemAPI = scope.ServiceProvider.GetRequiredService<IExternalSystemAPI>();
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

                            var success = await InformExternalSystemTheTenantIsUnavailableAsync(jobTask, productService, stoppingToken);

                            if (success)
                            {
                                await RemoveJobTaskAsync(jobTask, stoppingToken);
                            }
                            else
                            {

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

                    }
                    finally
                    {

                    }
                    taskIndex++;
                }
                Log($"#Cycle Execution Finished");
                cycleIndex++;
            }
        }
    }
}

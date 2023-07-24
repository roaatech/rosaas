using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.BackgroundServices.Workers.abstruct;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices.Workers
{
    public class UnavailableTenantChecker : BaseWorker
    {
        protected override TimeSpan _period { get; set; } = TimeSpan.FromSeconds(60 * 10);

        public UnavailableTenantChecker(ILogger<BackgroundServiceManager> logger,
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

                _backgroundWorkerStore.RefillUnavailableTenantTask();

                using var scope = _serviceScopeFactory.CreateScope();
                _dbContext = scope.ServiceProvider.GetRequiredService<IRosasDbContext>();
                _externalSystemAPI = scope.ServiceProvider.GetRequiredService<IExternalSystemAPI>();

                while (!_backgroundWorkerStore.UnavailableTenantsTasks.IsCompleted)
                {
                    try
                    {
                        Log($"#Try to take a job Task");

                        if (_backgroundWorkerStore.UnavailableTenantsTasks.TryTake(out var jobTask) &&
                            _backgroundWorkerStore.MakeSureIsNotRemoved(jobTask))
                        {
                            Log($"##Took the JobTask, for the tenant: [TenantId:{{0}}], [ProductId:{{1}}]", jobTask.TenantId, jobTask.ProductId);

                            var isAvailable = await CheckTenantHealthStatusAndRecordResultAsync(jobTask, stoppingToken);

                            if (isAvailable)
                            {
                                await RemoveUnavailableJobTaskAsync(jobTask, stoppingToken);

                                await AddInaccessibleJobTaskAsync(jobTask, stoppingToken);
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
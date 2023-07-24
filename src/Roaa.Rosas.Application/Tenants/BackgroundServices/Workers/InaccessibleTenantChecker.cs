using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Tenants.BackgroundServices.Workers.abstruct;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices.Workers
{
    public class InaccessibleTenantChecker : BaseWorker
    {
        protected override TimeSpan _period { get; set; } = TimeSpan.FromSeconds(60 * 1);

        public InaccessibleTenantChecker(ILogger<BackgroundServiceManager> logger,
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

                while (!_backgroundWorkerStore.InaccessibleTenantsTasks.IsCompleted)
                {
                    try
                    {
                        Log($"#Try to take a job Task");

                        if (_backgroundWorkerStore.InaccessibleTenantsTasks.TryTake(out var jobTask) &&
                                _backgroundWorkerStore.MakeSureIsNotRemoved(jobTask))
                        {
                            await Task.Run(async () =>
                             {
                                 bool isAvailable = false;

                                 using PeriodicTimer subTimer = new PeriodicTimer(TimeSpan.FromSeconds(60));

                                 int counter = 1;

                                 using var scope = _serviceScopeFactory.CreateScope();
                                 _dbContext = scope.ServiceProvider.GetRequiredService<IRosasDbContext>();
                                 _externalSystemAPI = scope.ServiceProvider.GetRequiredService<IExternalSystemAPI>();


                                 while (counter < 3 && await subTimer.WaitForNextTickAsync(stoppingToken))
                                 {
                                     Log($"##-[{{0}}]Took the JobTask, for the tenant: [TenantId:{{1}}], [ProductId:{{2}}]", counter, jobTask.TenantId, jobTask.ProductId);

                                     isAvailable = await CheckTenantHealthStatusAndRecordResultAsync(jobTask, stoppingToken);

                                     counter++;
                                 }
                                 await RemoveJobTaskAsync(jobTask, stoppingToken);

                                 if (isAvailable)
                                 {
                                 }
                                 else
                                 {
                                     await AddInformerJobTaskAsync(jobTask, stoppingToken);

                                     await AddUnavailableJobTaskAsync(jobTask, stoppingToken);
                                 }
                             });
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
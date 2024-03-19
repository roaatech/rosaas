using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Services;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices.abstruct;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices
{
    public class InaccessibleTenantHealthCheckWorker : BaseWorker, IInaccessibleTenantChecker
    {
        protected override TimeSpan _period { get; set; }

        public InaccessibleTenantHealthCheckWorker(ILogger<InaccessibleTenantHealthCheckWorker> logger,
                                          IServiceScopeFactory serviceScopeFactory,
                                          BackgroundServicesStore backgroundWorkerStore)
            : base(logger, serviceScopeFactory, backgroundWorkerStore)
        {
            SetPeriod();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log("Started. It's will execute its work every [{0}] seconds", _period.TotalSeconds);

            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            {
                Log($"#Cycle Execution Started");

                await CheckAsync(cancellationToken);

                Log($"#Cycle Execution Finished");
                cycleIndex++;
            }
        }





        private async Task CheckAsync(CancellationToken cancellationToken = default)
        {
            while (!_backgroundWorkerStore.InaccessibleTenantsTasks.IsCompleted)
            {
                try
                {
                    Log($"#Try to take a job Task");

                    if (_backgroundWorkerStore.InaccessibleTenantsTasks.TryTake(out var jobTask) && _backgroundWorkerStore.MakeSureIsNotRemoved(jobTask))
                    {
                        tenantId = jobTask.TenantId;
                        productId = jobTask.ProductId;
                        bool isAvailable = false;

                        using PeriodicTimer subTimer = new PeriodicTimer(_period);

                        int counter = 1;

                        using var scope = _serviceScopeFactory.CreateScope();
                        _tenantHealthCheckService = scope.ServiceProvider.GetRequiredService<ITenantHealthCheckService>();


                        while (counter < _backgroundWorkerStore.Settings.TimesNumberBeforeInformExternalSys && await subTimer.WaitForNextTickAsync(cancellationToken))
                        {
                            Log($"##-[{{0}}]Took the JobTask, for the tenant: [TenantId:{{1}}], [ProductId:{{2}}]", counter, jobTask.TenantId, jobTask.ProductId);

                            isAvailable = await CheckTenantHealthStatusAndRecordResultAsync(jobTask, cancellationToken);
                            if (isAvailable)
                            {
                                await _tenantHealthCheckService.PublishTenantProcessingCompletedEventAsync(jobTask, TenantProcessType.HealthyStatus, cancellationToken);
                            }
                            else
                            {
                                await _tenantHealthCheckService.PublishTenantProcessingCompletedEventAsync(jobTask, TenantProcessType.UnhealthyStatus, cancellationToken);
                            }
                            counter++;
                        }
                        await _tenantHealthCheckService.RemoveInaccessibleJobTaskTasks(jobTask, cancellationToken);

                        if (isAvailable)
                        {
                            await _tenantHealthCheckService.AddAvailableTenantTaskAsync(jobTask, cancellationToken);
                        }
                        else
                        {
                            await _tenantHealthCheckService.AddInformerJobTaskAsync(jobTask, cancellationToken);


                            await _tenantHealthCheckService.AddUnavailableJobTaskAsync(jobTask, cancellationToken);


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

        public async Task RestartAsync(CancellationToken token = default)
        {
            var timePeriod = ToSeconds(_backgroundWorkerStore.Settings.InaccessibleCheckTimePeriod);

            if (IsPeriodUpdated(timePeriod))
            {
                SetPeriod();

                Log("Will be restarted after its time period updated. It's will execute its work every [{0}] seconds", timePeriod);

                await base.StartAsync(token);
            }
        }

        public void SetPeriod()
        {
            _period = TimeSpan.FromMinutes(_backgroundWorkerStore.Settings.InaccessibleCheckTimePeriod);
        }

    }

    public interface IInaccessibleTenantChecker
    {
        Task RestartAsync(CancellationToken token = default);
    }
}
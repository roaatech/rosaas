﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Services;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices.abstruct;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices
{
    public class UnavailableTenantHealthCheckWorker : BaseWorker, IUnavailableTenantChecker
    {
        protected override TimeSpan _period { get; set; }

        public UnavailableTenantHealthCheckWorker(ILogger<UnavailableTenantHealthCheckWorker> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  BackgroundServicesStore backgroundWorkerStore)
            : base(logger, serviceScopeFactory, backgroundWorkerStore)
        {
            SetPeriod();
        }



        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log("Started. It's will execute its work every [{0}] seconds", _period.TotalSeconds);

            using PeriodicTimer timer = new PeriodicTimer(_period);

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
            _backgroundWorkerStore.RefillUnavailableTenantTask();

            using var scope = _serviceScopeFactory.CreateScope();
            _tenantHealthCheckService = scope.ServiceProvider.GetRequiredService<ITenantHealthCheckService>();

            while (!_backgroundWorkerStore.UnavailableTenantsTasks.IsCompleted)
            {
                try
                {
                    Log($"#Try to take a job Task");

                    if (_backgroundWorkerStore.UnavailableTenantsTasks.TryTake(out var jobTask) &&
                        _backgroundWorkerStore.MakeSureIsNotRemoved(jobTask))
                    {
                        tenantId = jobTask.TenantId;
                        productId = jobTask.ProductId;
                        Log($"##Took the JobTask, for the tenant: [TenantId:{{0}}], [ProductId:{{1}}]", jobTask.TenantId, jobTask.ProductId);

                        var isAvailable = await CheckTenantHealthStatusAndRecordResultAsync(jobTask, cancellationToken);

                        if (isAvailable)
                        {
                            await _tenantHealthCheckService.PublishTenantProcessingCompletedEventAsync(jobTask, TenantProcessType.HealthyStatus, cancellationToken);

                            await _tenantHealthCheckService.RemoveUnavailableJobTaskAsync(jobTask, cancellationToken);

                            await _tenantHealthCheckService.AddInaccessibleJobTaskAsync(jobTask, cancellationToken);
                        }
                        else
                        {
                            await _tenantHealthCheckService.UpdateTenantProcessHistoryAsHealthCheckStatusAsync(jobTask, cancellationToken);
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
            var timePeriod = ToSeconds(_backgroundWorkerStore.Settings.UnavailableCheckTimePeriod);

            if (IsPeriodUpdated(timePeriod))
            {
                SetPeriod();

                Log("Will be restarted after its time period updated. It's will execute its work every [{0}] seconds", timePeriod);

                await base.StartAsync(token);
            }
        }

        public void SetPeriod()
        {
            _period = TimeSpan.FromMinutes(_backgroundWorkerStore.Settings.UnavailableCheckTimePeriod);
        }
    }

    public interface IUnavailableTenantChecker
    {
        Task RestartAsync(CancellationToken token = default);
    }
}
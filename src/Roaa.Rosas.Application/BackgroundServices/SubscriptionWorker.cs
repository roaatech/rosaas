using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.RenwalSubscription;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Commands.HandleExpiredSubscription;
using Roaa.Rosas.Application.Services.Management.SubscriptionTrials.Commands.UpgradeTrialSubscriptionToStandard;
using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.BackgroundServices
{
    public interface ISubscriptionWorker
    {
        Task RestartAsync(CancellationToken token = default);
    }
    public class SubscriptionWorker : BackgroundService, ISubscriptionWorker
    {
        protected readonly ILogger<SubscriptionWorker> _logger;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;
        protected TimeSpan _period { get; set; }



        public SubscriptionWorker(ILogger<SubscriptionWorker> logger,
                                  IServiceScopeFactory serviceScopeFactory,
                                  IMediator mediator)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _mediator = mediator;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} Background Service Started. It's will execute its work every [{1}] hours.", GetType().Name, _period.TotalHours);

            using var scope = _serviceScopeFactory.CreateScope();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();

            var settings = (await settingService.LoadSettingAsync<SubscriptionSettings>(cancellationToken)).Data;

            //  _period = TimeSpan.FromHours(settings.SubscriptionWorkerTimePeriod);
            _period = TimeSpan.FromHours(1);

            using PeriodicTimer timer = new PeriodicTimer(_period);

            do
            {
                var index = DateTime.UtcNow.Ticks;
                _logger.LogInformation("{0} [#{1}] Cycle Execution Started. [{2}]", GetType().Name, index, DateTime.UtcNow.ToString("O"));

                await DoAsync(cancellationToken);

                _logger.LogInformation("{0} [#{1}] Cycle Execution Finished. [{2}]", GetType().Name, index, DateTime.UtcNow.ToString("O"));
            }
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken));



        }


        protected async Task DoAsync(CancellationToken cancellationToken)
        {
            try
            {
                var startingMsg = "The {0} as background service started running the {1} task.";
                var completingMsg = "The {0} as background service completed the {1} task.";
                var rangeInHoursBetweenDates = 100000;

                Task upgradeTrialSubscriptionToStandardTask = Task.Run(async () =>
                {
                    _logger.LogInformation(startingMsg, nameof(SubscriptionWorker), nameof(upgradeTrialSubscriptionToStandardTask));
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new UpgradeTrialSubscriptionToStandardCommand(rangeInHoursBetweenDates), cancellationToken);
                    _logger.LogInformation(completingMsg, nameof(SubscriptionWorker), nameof(upgradeTrialSubscriptionToStandardTask));
                });

                Task renewalSubscriptionTask = Task.Run(async () =>
                {
                    _logger.LogInformation(startingMsg, nameof(SubscriptionWorker), nameof(renewalSubscriptionTask));
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new RenwalSubscriptionCommand(rangeInHoursBetweenDates), cancellationToken);
                    _logger.LogInformation(completingMsg, nameof(SubscriptionWorker), nameof(renewalSubscriptionTask));
                });

                // Wait for both tasks to complete
                await Task.WhenAll(upgradeTrialSubscriptionToStandardTask, renewalSubscriptionTask);


                Task handleExpiredSubscriptionTask = Task.Run(async () =>
                {
                    _logger.LogInformation(startingMsg, nameof(SubscriptionWorker), nameof(handleExpiredSubscriptionTask));
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new HandleExpiredSubscriptionCommand(rangeInHoursBetweenDates), cancellationToken);
                    _logger.LogInformation(completingMsg, nameof(SubscriptionWorker), nameof(handleExpiredSubscriptionTask));
                });

                Task resetSubscriptionsFeaturesTask = Task.Run(async () =>
                {
                    _logger.LogInformation(startingMsg, nameof(SubscriptionWorker), nameof(resetSubscriptionsFeaturesTask));
                    using var scope = _serviceScopeFactory.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    // TODO : review implementation
                    // Reset Subscriptions Features
                    await subscriptionService.ResetSubscriptionsFeaturesAsync();
                    _logger.LogInformation(completingMsg, nameof(SubscriptionWorker), nameof(resetSubscriptionsFeaturesTask));
                });

                await Task.WhenAll(new[] { upgradeTrialSubscriptionToStandardTask,
                                           renewalSubscriptionTask,
                                           handleExpiredSubscriptionTask,
                                           resetSubscriptionsFeaturesTask });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the subscription schedule.");
            }
            finally
            {
            }
        }

        public async Task RestartAsync(CancellationToken cancellationToken = default)
        {
            return;
            using var scope = _serviceScopeFactory.CreateScope();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();

            var settings = (await settingService.LoadSettingAsync<SubscriptionSettings>(cancellationToken)).Data;

            if (_period.TotalHours != settings.SubscriptionWorkerTimePeriod)
            {
                SetPeriod(settings.SubscriptionWorkerTimePeriod);

                _logger.LogInformation("{0} Background Service Will be restarted after its time period updated. It's will execute its work every [{1}] hours",
                                        GetType().Name,
                                        _period.TotalHours);

                await base.StartAsync(cancellationToken);
            }
        }

        public void SetPeriod(int period)
        {
            _period = TimeSpan.FromMinutes(period);
        }
    }

}
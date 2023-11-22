using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
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
        protected TimeSpan _period { get; set; }



        public SubscriptionWorker(ILogger<SubscriptionWorker> logger,
                                  IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0} Background Service Started. It's will execute its work every [{1}] hours.", GetType().Name, _period.TotalHours);

            using var scope = _serviceScopeFactory.CreateScope();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();

            var settings = (await settingService.LoadSettingAsync<SubscriptionSettings>(cancellationToken)).Data;

            _period = TimeSpan.FromHours(settings.SubscriptionWorkerTimePeriod);

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
                Task task1 = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    await subscriptionService.ResetSubscriptionsFeaturesAsync();
                });

                Task task2 = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    await subscriptionService.TryToExtendOrSuspendSubscriptionsAsync();
                });

                Task task3 = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();
                    var settings = (await settingService.LoadSettingAsync<SubscriptionSettings>(cancellationToken)).Data;

                    await subscriptionService.DeactivateSubscriptionDueToNonPaymentAsync(settings.AllowedPeriodTimeBeforeDeactivatingSubscriptionforNonPayment);
                });
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
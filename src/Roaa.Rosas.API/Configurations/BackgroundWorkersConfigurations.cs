using Roaa.Rosas.Application.BackgroundServices;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.BackgroundServices;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class BackgroundWorkersConfigurations
    {
        public static void AdBackgroundWorkersConfigurations(this IServiceCollection services,
                                                           IConfiguration configuration,
                                                           IWebHostEnvironment environment,
                                                           RootOptions rootOptions)
        {
            services.AddSingleton<IAvailableTenantChecker, AvailableTenantHealthCheckWorker>();
            services.AddSingleton<IInaccessibleTenantChecker, InaccessibleTenantHealthCheckWorker>();
            services.AddSingleton<IUnavailableTenantChecker, UnavailableTenantHealthCheckWorker>();
            services.AddSingleton<ISubscriptionWorker, SubscriptionWorker>();
            services.AddSingleton<BackgroundServicesStore>();
            services.AddScoped<BackgroundServiceManager>();

            services.AddHostedService<InaccessibleTenantHealthCheckWorker>();
            services.AddHostedService<AvailableTenantHealthCheckWorker>();
            services.AddHostedService<UnavailableTenantHealthCheckWorker>();
            services.AddHostedService<InformerHealthCheckWorker>();
            services.AddHostedService<SubscriptionWorker>();
        }


    }
}





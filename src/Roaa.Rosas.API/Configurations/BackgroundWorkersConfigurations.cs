using Roaa.Rosas.Application.Tenants.BackgroundServices;
using Roaa.Rosas.Application.Tenants.HealthCheckStatus.BackgroundServices;
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
            services.AddSingleton<IAvailableTenantChecker, AvailableTenantChecker>();
            services.AddSingleton<IInaccessibleTenantChecker, InaccessibleTenantChecker>();
            services.AddSingleton<IUnavailableTenantChecker, UnavailableTenantChecker>();
            services.AddSingleton<BackgroundServicesStore>();
            services.AddScoped<BackgroundServiceManager>();

            services.AddHostedService<InaccessibleTenantChecker>();
            services.AddHostedService<AvailableTenantChecker>();
            services.AddHostedService<UnavailableTenantChecker>();
            services.AddHostedService<Informer>();
        }


    }
}





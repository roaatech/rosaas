using Roaa.Rosas.Application.Tenants.BackgroundServices;
using Roaa.Rosas.Application.Tenants.BackgroundServices.Workers;
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
            services.AddSingleton<BackgroundWorkerStore>();
            services.AddSingleton<BackgroundServiceManager>();

            services.AddHostedService<InaccessibleTenantChecker>();
            services.AddHostedService<AvailableTenantChecker>();
            services.AddHostedService<UnavailableTenantChecker>();
            services.AddHostedService<Informer>();
        }


    }
}





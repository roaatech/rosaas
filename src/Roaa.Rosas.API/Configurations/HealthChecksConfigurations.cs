using Microsoft.Extensions.Diagnostics.HealthChecks;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Framework.HealthCheckers;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class HealthChecksConfigurations
    {
        public static IHealthChecksBuilder AddIdentityServiceHealthChecks(this IHealthChecksBuilder healthChecksBuilder)
        {
            return healthChecksBuilder.AddCheck<IdentitySqlDbHealthChecker>("IdentityDb-Check", HealthStatus.Unhealthy, new string[] { "readiness" }); ;
        }

        public static void AddHealthCheckers(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    IWebHostEnvironment environment,
                                                    RootOptions rootOptions)
        {
            services.AddSingleton<IHealthCheck, IdentitySqlDbHealthChecker>();
        }
    }
}

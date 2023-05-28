using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Authorization
{
    public static class Startup
    {
        public static void AddAuthorizationConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityContextService, HttpContextService>();

        }

    }
}

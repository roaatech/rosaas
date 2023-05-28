using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Common.ApiConfiguration;

namespace Roaa.Rosas.Common
{
    public static class Startup
    {
        public static void AddCommonConfigurations(this IServiceCollection services)
        {
            services.RegisterApiConfigurationService();
        }

        public static void RegisterApiConfigurationService(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IApiConfigurationService<>), typeof(ApiConfigurationService<>));
        }
    }
}
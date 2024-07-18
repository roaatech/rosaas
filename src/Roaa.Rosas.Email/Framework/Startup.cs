using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roaa.RoSaaS.Email.Framework.Configurations;

namespace Roaa.RoSaaS.Email.Framework;

public static class Startup
{
    public static void AddEmailServiceConfigurations(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        var rootOptions = services.AddOptionsConfigurations(configuration);
        services.AddApplicationServicesConfigurations(configuration, env, rootOptions);
        services.AddMessageBrokerConfigurations(configuration, env, rootOptions);
    }
}
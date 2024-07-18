using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roaa.RoSaaS.Email.Domain.Options;

namespace Roaa.RoSaaS.Email.Framework.Configurations;

public static class OptionsConfigurations
{
    public static RootOptions AddOptionsConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var rootOptions = configuration.GetSection(RootOptions.Section).Get<RootOptions>();


        services.Configure<SendGridOptions>(options =>
        {
            configuration.GetSection(RootOptions.Section).GetSection(SendGridOptions.Section).Bind(options);
        });


        services.Configure<RootOptions>(options =>
        {
            configuration.GetSection(RootOptions.Section).Bind(options);
        });

        return rootOptions;
    }
}
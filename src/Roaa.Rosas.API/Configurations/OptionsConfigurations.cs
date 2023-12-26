using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class OptionsConfigurations
    {
        public static RootOptions AddOptionsConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            var rootOptions = configuration.Get<RootOptions>();
            rootOptions.General = configuration.GetSection(GeneralOptions.Section).Get<GeneralOptions>();

            services.Configure<IdentityServerOptions>(options =>
            {
                configuration.GetSection(IdentityServerOptions.Section).Bind(options);
            });

            services.Configure<ConnectionStringsOptions>(options =>
            {
                configuration.GetSection(ConnectionStringsOptions.Section).Bind(options);
            });

            services.Configure<GeneralOptions>(options =>
            {
                configuration.GetSection(GeneralOptions.Section).Bind(options);
            });

            services.Configure<PaymentOptions>(options =>
            {
                configuration.GetSection(PaymentOptions.Section).Bind(options);
            });

            return rootOptions;
        }
    }

}





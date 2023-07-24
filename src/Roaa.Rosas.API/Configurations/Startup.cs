using Microsoft.AspNetCore.Identity;
using Roaa.Rosas.Auditing;
using Roaa.Rosas.Framework.Configurations;

namespace Roaa.Rosas.API.Configurations
{
    public static class Startup
    {
        public static void AddRosasServiceConfigurations(this IServiceCollection services,
                                                                 IConfiguration configuration,
                                                                 IWebHostEnvironment env)
        {



            var rootOptions = services.AddOptionsConfigurations(configuration);

            services.AddDbContextConfigurations(configuration, env, rootOptions);
            services.AddIdentityConfigurations(configuration, env, rootOptions);
            services.AddIdentityServer4Configurations(configuration, env, rootOptions);
            services.AddApiAuthenticationConfigurations(configuration, env, rootOptions);
            services.AddApiAuthorizationPolicies(configuration);
            services.AddApplicationServicesConfigurations(configuration, env, rootOptions);
            services.AddHealthCheckers(configuration, env, rootOptions);
            services.AdBackgroundWorkersConfigurations(configuration, env, rootOptions);
            services.AddAudit(rootOptions.ConnectionStrings.IdentityDb);
            //configure identity tokens expiry life-time
            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromMinutes(30));
        }
    }
}




//dotnet ef migrations add "InitIdentityDbMigration" --context RosasDbContext --project src\Roaa.Rosas.Infrastructure --startup-project src\Roaa.Rosas.API --output-dir Persistence\Migrations\Identity
//dotnet ef migrations add "InitIdentityServerConfigurationDbMigration" --context IdentityServerConfigurationDbContext --project src\Roaa.Rosas.Infrastructure --startup-project src\Roaa.Rosas.API --output-dir Persistence\Migrations\IdentityServerConfiguration
//dotnet ef migrations add "InitIdentityServerPersistedGrantDbMigration" --context IdentityServerPersistedGrantDbContext --project src\Roaa.Rosas.Infrastructure --startup-project src\Roaa.Rosas.API --output-dir Persistence\Migrations\IdentityServerGrants
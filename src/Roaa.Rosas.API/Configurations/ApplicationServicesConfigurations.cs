using MediatR;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Infrastructure.Persistence.DbContexts;
using Roaa.Rosas.Infrastructure.Persistence.Interceptors;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.Identity;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.IdentityServer4;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class ApplicationServicesConfigurations
    {
        public static void AddApplicationServicesConfigurations(this IServiceCollection services,
                                                                      IConfiguration configuration,
                                                                      IWebHostEnvironment environment,
                                                                      RootOptions rootOptions)
        {

            services.AddMediatR(typeof(IInternalEventHandler<>));
            services.AddScoped<IRosasIdentityDbContext, RosasIdentityDbContext>();
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<IdentityDbInitialiser>();

            services.AddScoped<IIdentityServerConfigurationDbContext, IdentityServerConfigurationDbContext>();
            services.AddScoped<IIdentityServerPersistedGrantDbContext, IdentityServerPersistedGrantDbContext>();
            services.AddScoped<IdentityServerConfigurationDbInitialiser>();

            services.AddScoped<IAuthTokenService, JWTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
        }
    }
}





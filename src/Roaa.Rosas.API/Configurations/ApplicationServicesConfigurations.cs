using Roaa.Rosas.Application.ExternalSystemsAPI;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.JWT;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth;
using Roaa.Rosas.Application.Services.Management.Features;
using Roaa.Rosas.Application.Services.Management.PlanFeatures;
using Roaa.Rosas.Application.Services.Management.PlanPrices;
using Roaa.Rosas.Application.Services.Management.Plans;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Services;
using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Settings;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Infrastructure.Persistence.DbContexts;
using Roaa.Rosas.Infrastructure.Persistence.Interceptors;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.Identity;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.IdentityServer4;
using Roaa.Rosas.Infrastructure.Persistence.SeedData.Management;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class ApplicationServicesConfigurations
    {
        public static void AddApplicationServicesConfigurations(this IServiceCollection services,
                                                                      IConfiguration configuration,
                                                                      IWebHostEnvironment environment,
                                                                      RootOptions rootOptions)
        {
            #region Identity
            services.AddScoped<IRosasDbContext, RosasDbContext>();
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<IdentityDbInitialiser>();

            services.AddScoped<IAuthTokenService, JWTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            #endregion


            #region IdentityServer4
            services.AddScoped<IIdentityServerConfigurationDbContext, IdentityServerConfigurationDbContext>();
            services.AddScoped<IIdentityServerPersistedGrantDbContext, IdentityServerPersistedGrantDbContext>();
            services.AddScoped<IdentityServerConfigurationDbInitialiser>();

            services.AddScoped<IClientAuthService, ClientAuthService>();
            #endregion


            #region Management 
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantWorkflow, TenantWorkflow>();
            services.AddScoped<ManagementDbInitialiser>();
            #endregion


            services.AddScoped<IExternalSystemAPI, ExternalSystemAPI>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IPlanFeatureService, PlanFeatureService>();
            services.AddScoped<IPlanPriceService, PlanPriceService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ITenantHealthCheckService, TenantHealthCheckService>();
            services.AddScoped<ITenantHealthCheckSettingsService, TenantHealthCheckSettingsService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISpecificationService, SpecificationService>();

            services.AddMediatRAServices();

        }
    }
}





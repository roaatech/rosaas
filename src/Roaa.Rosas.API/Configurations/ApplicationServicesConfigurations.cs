﻿using Roaa.Rosas.Application.ExternalSystemsAPI;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.JWT;
using Roaa.Rosas.Application.Payment;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth;
using Roaa.Rosas.Application.Services.IdentityServer4.Clients;
using Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets;
using Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges;
using Roaa.Rosas.Application.Services.Management.Features;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.PlanFeatures;
using Roaa.Rosas.Application.Services.Management.PlanPrices;
using Roaa.Rosas.Application.Services.Management.Plans;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.SubscriptionAutoRenewals;
using Roaa.Rosas.Application.Services.Management.SubscriptionPlansChanging;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.SubscriptionTrials;
using Roaa.Rosas.Application.Services.Management.TenantCreationRequests;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Services;
using Roaa.Rosas.Application.Services.Management.TenantHealthChecks.Settings;
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
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IClientSecretService, ClientSecretService>();
            #endregion


            #region Management 
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantCreationRequestService, TenantCreationRequestService>();
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
            services.AddScoped<ISubscriptionAutoRenewalService, SubscriptionAutoRenewalService>();
            services.AddScoped<ISubscriptionPlanChangingService, SubscriptionPlanChangingService>();
            services.AddScoped<ISpecificationService, SpecificationService>();
            services.AddScoped<IEntityAdminPrivilegeService, EntityAdminPrivilegeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IGenericAttributeService, GenericAttributeService>();
            services.AddScoped<ITrialProcessingService, TrialProcessingService>();


            services.AddMediatRAServices();



            services.AddPaymentServicesConfigurations(rootOptions);

        }
    }
}





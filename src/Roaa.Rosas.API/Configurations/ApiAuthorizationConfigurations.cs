using Microsoft.AspNetCore.Authorization;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class ApiAuthorizationConfigurations
    {
        public static void AddApiAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorization(configure =>
            {

                #region  SuperAdmin  
                configure.AddPolicy(AuthPolicy.SuperAdmin, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy());
                });
                #endregion

                configure.AddPolicy(AuthPolicy.Identity.Account, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy(),
                                                                                UserType.TenantOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Identity.ClientCredential, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Settings, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Tenants, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy(),
                                                                                UserType.TenantOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Products, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Features, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.PlanFeatures, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.PlanPrices, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Plans, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Specifications, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Subscriptions, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy(),
                                                                                UserType.ProductOwner.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.Workflow, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy());
                });

                configure.AddPolicy(AuthPolicy.Management.GeneralPlans, builder =>
                {
                    builder.RequireScope(SystemConsts.Scopes.Api);
                    builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, UserType.SuperAdmin.ToSnakeCaseNamingStrategy());
                });

                #region  SuperAdmin  
                configure.AddPolicy(AuthPolicy.ExternalSystem, builder =>
                        {
                            builder.RequireScope(SystemConsts.Scopes.ExternalSystem);
                            builder.RequireClaim(SystemConsts.Clients.Claims.ClaimProductId);
                            builder.RequireClaim(SystemConsts.Clients.Claims.ClaimType, SystemConsts.Clients.Claims.ExternalSystem);
                        });
                #endregion
            });

        }
    }
}





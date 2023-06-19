using Microsoft.AspNetCore.Authorization;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Authorization.Utilities;

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
                    builder.RequireScope(SystemConsts.Scopes.SuperAdmin);
                });
                #endregion


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





using Microsoft.AspNetCore.Authorization;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class ApiAuthorizationConfigurations
    {
        public const string ClaimType = "specification";
        public static void AddApiAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorization(configure =>
            {

                #region  SuperAdmin  
                configure.AddPolicy(AuthPolicy.SuperAdmin, builder =>
                {
                    builder.RequireScope("SuperAdminScope");
                });
                #endregion
            });

        }
    }
}





using Microsoft.AspNetCore.Authentication.JwtBearer;
using Roaa.Rosas.Domain.Models.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class ApiAuthenticationConfigurations
    {
        public static void AddApiAuthenticationConfigurations(this IServiceCollection services,
                                                                IConfiguration configuration,
                                                                IWebHostEnvironment environment,
                                                                RootOptions rootOptions)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
            .AddJwtBearer(options =>
            {
                options.Authority = rootOptions.IdentityServer.Url;
                options.Audience = rootOptions.IdentityServer.ApiName;
                options.RequireHttpsMetadata = rootOptions.IdentityServer.RequireHttpsMetadata;
                options.MapInboundClaims = false;

            });

        }
    }
}
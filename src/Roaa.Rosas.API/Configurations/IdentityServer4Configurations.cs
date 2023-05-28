using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class IdentityServer4Configurations
    {
        private const string IdS4cPrefix = "IdS4c_";
        private const string IdS4gPrefix = "IdS4g_";
        public static void AddIdentityServer4Configurations(this IServiceCollection services,
                                                                 IConfiguration configuration,
                                                                 IWebHostEnvironment environment,
                                                                 RootOptions rootOptions)
        {
            var builder = services.AddIdentityServer();

            if (rootOptions.IdentityServer.UseInMemoryDatabase)
            {
                builder.AddInMemoryIdentityResources(IdentityServer4Config.GetIdentityResources())
                        .AddInMemoryApiResources(IdentityServer4Config.GetApiResources())
                        .AddInMemoryApiScopes(IdentityServer4Config.GetApiScopes())
                        .AddInMemoryClients(IdentityServer4Config.GetClients(rootOptions.IdentityServer.Url))
                        .AddTestUsers(IdentityServer4Config.GetTestUsers())
                        .AddDeveloperSigningCredential();
                // .Services.AddTransient<IProfileService, ProfileService>();
            }
            else
            {
                if (environment.IsProductionEnvironment())
                {
                    var jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(rootOptions.IdentityServer.Jwk);

                    builder.AddSigningCredential(jwk, jwk.Alg);
                }
                else
                {
                    // Adding Developer Signing Credential, This will generate tempkey.rsa file 
                    builder.AddDeveloperSigningCredential();
                }



                builder.AddAspNetIdentity<User>()

                     // this adds the config data from DB (clients, resources)
                     .AddConfigurationStore<IdentityServerConfigurationDbContext>(options =>
                     {
                         options.ConfigureDbContext = builder =>
                             builder.UseMySql(rootOptions.ConnectionStrings.IdS4ConfigurationDb, new MySqlServerVersion(new Version()), config =>
                             {
                                 config.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                                                               rootOptions.General.UseSingleDatabase ? "ids4configuration" : null);
                             });
                         options.IdentityResource = new TableConfiguration($"{IdS4cPrefix}IdentityResources");
                         options.IdentityResourceClaim = new TableConfiguration($"{IdS4cPrefix}IdentityResourceClaims");
                         options.IdentityResourceProperty = new TableConfiguration($"{IdS4cPrefix}IdentityResourceProperties");
                         options.ApiResource = new TableConfiguration($"{IdS4cPrefix}ApiResources");
                         options.ApiResourceSecret = new TableConfiguration($"{IdS4cPrefix}ApiResourceSecrets");
                         options.ApiResourceScope = new TableConfiguration($"{IdS4cPrefix}ApiResourceScopes");
                         options.ApiResourceClaim = new TableConfiguration($"{IdS4cPrefix}ApiResourceClaims");
                         options.ApiResourceProperty = new TableConfiguration($"{IdS4cPrefix}ApiResourceProperties");
                         options.Client = new TableConfiguration($"{IdS4cPrefix}Clients");
                         options.ClientGrantType = new TableConfiguration($"{IdS4cPrefix}ClientGrantTypes");
                         options.ClientRedirectUri = new TableConfiguration($"{IdS4cPrefix}ClientRedirectUris");
                         options.ClientPostLogoutRedirectUri = new TableConfiguration($"{IdS4cPrefix}ClientPostLogoutRedirectUris");
                         options.ClientScopes = new TableConfiguration($"{IdS4cPrefix}ClientScopes");
                         options.ClientSecret = new TableConfiguration($"{IdS4cPrefix}ClientSecrets");
                         options.ClientClaim = new TableConfiguration($"{IdS4cPrefix}ClientClaims");
                         options.ClientIdPRestriction = new TableConfiguration($"{IdS4cPrefix}ClientIdPRestrictions");
                         options.ClientCorsOrigin = new TableConfiguration($"{IdS4cPrefix}ClientCorsOrigins");
                         options.ClientProperty = new TableConfiguration($"{IdS4cPrefix}ClientProperties");
                         options.ApiScope = new TableConfiguration($"{IdS4cPrefix}ApiScopes");
                         options.ApiScopeClaim = new TableConfiguration($"{IdS4cPrefix}ApiScopeClaims");
                         options.ApiScopeProperty = new TableConfiguration($"{IdS4cPrefix}ApiScopeProperties");
                     })

                    // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore<IdentityServerPersistedGrantDbContext>(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseMySql(rootOptions.ConnectionStrings.IdS4PersistedGrantDb, new MySqlServerVersion(new Version()), config =>
                            {
                                config.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                                                              rootOptions.General.UseSingleDatabase ? "ids4persistedgrant" : null);
                            });
                        options.PersistedGrants = new TableConfiguration($"{IdS4gPrefix}PersistedGrants");
                        options.DeviceFlowCodes = new TableConfiguration($"{IdS4gPrefix}DeviceCodes");

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.TokenCleanupInterval = 30;
                    });
            }


            //   services.AddTransient<IProfileService, ProfileService>();
        }
    }
}





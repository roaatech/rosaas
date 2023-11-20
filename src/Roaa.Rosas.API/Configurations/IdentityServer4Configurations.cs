using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class IdentityServer4Configurations
    {
        private const string IdS4cPrefix = "Ids4";
        private const string IdS4gPrefix = "Ids4";
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
                         options.IdentityResource = new TableConfiguration($"{IdS4cPrefix}IdentityResources".ToTableNamingStrategy());
                         options.IdentityResourceClaim = new TableConfiguration($"{IdS4cPrefix}IdentityResourceClaims".ToTableNamingStrategy());
                         options.IdentityResourceProperty = new TableConfiguration($"{IdS4cPrefix}IdentityResourceProperties".ToTableNamingStrategy());
                         options.ApiResource = new TableConfiguration($"{IdS4cPrefix}ApiResources".ToTableNamingStrategy());
                         options.ApiResourceSecret = new TableConfiguration($"{IdS4cPrefix}ApiResourceSecrets".ToTableNamingStrategy());
                         options.ApiResourceScope = new TableConfiguration($"{IdS4cPrefix}ApiResourceScopes".ToTableNamingStrategy());
                         options.ApiResourceClaim = new TableConfiguration($"{IdS4cPrefix}ApiResourceClaims".ToTableNamingStrategy());
                         options.ApiResourceProperty = new TableConfiguration($"{IdS4cPrefix}ApiResourceProperties".ToTableNamingStrategy());
                         options.Client = new TableConfiguration($"{IdS4cPrefix}Clients".ToTableNamingStrategy());
                         options.ClientGrantType = new TableConfiguration($"{IdS4cPrefix}ClientGrantTypes".ToTableNamingStrategy());
                         options.ClientRedirectUri = new TableConfiguration($"{IdS4cPrefix}ClientRedirectUris".ToTableNamingStrategy());
                         options.ClientPostLogoutRedirectUri = new TableConfiguration($"{IdS4cPrefix}ClientPostLogoutRedirectUris".ToTableNamingStrategy());
                         options.ClientScopes = new TableConfiguration($"{IdS4cPrefix}ClientScopes".ToTableNamingStrategy());
                         options.ClientSecret = new TableConfiguration($"{IdS4cPrefix}ClientSecrets".ToTableNamingStrategy());
                         options.ClientClaim = new TableConfiguration($"{IdS4cPrefix}ClientClaims".ToTableNamingStrategy());
                         options.ClientIdPRestriction = new TableConfiguration($"{IdS4cPrefix}ClientIdPRestrictions".ToTableNamingStrategy());
                         options.ClientCorsOrigin = new TableConfiguration($"{IdS4cPrefix}ClientCorsOrigins".ToTableNamingStrategy());
                         options.ClientProperty = new TableConfiguration($"{IdS4cPrefix}ClientProperties".ToTableNamingStrategy());
                         options.ApiScope = new TableConfiguration($"{IdS4cPrefix}ApiScopes".ToTableNamingStrategy());
                         options.ApiScopeClaim = new TableConfiguration($"{IdS4cPrefix}ApiScopeClaims".ToTableNamingStrategy());
                         options.ApiScopeProperty = new TableConfiguration($"{IdS4cPrefix}ApiScopeProperties".ToTableNamingStrategy());
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
                        options.PersistedGrants = new TableConfiguration($"{IdS4gPrefix}PersistedGrants".ToTableNamingStrategy());
                        options.DeviceFlowCodes = new TableConfiguration($"{IdS4gPrefix}DeviceCodes".ToTableNamingStrategy());

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.TokenCleanupInterval = 30;
                    });
            }


            //   services.AddTransient<IProfileService, ProfileService>();
        }
    }
}





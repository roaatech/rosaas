using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Roaa.Rosas.Application.IdentityServer4
{
    public class IdentityServer4Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //http://docs.identityserver.io/en/latest/topics/resources.html#refapiresources
            //The OpenID Connect specification suggests a couple of standard scope name to claim type mappings 
            //that might be useful to you for inspiration, but you can freely design them yourself.
            //https://openid.net/specs/openid-connect-core-1_0.html#ScopeClaims
            return new IdentityResource[]
            {
                //opendid is mandatory, the openid scope will tell the identity service
                //to return the sub claim = (subject id) = (user id) in the identity token
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                // You may add other identity resources like address,phone... etc
                //new IdentityResources.Address()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            // The System microservice names that we need to protect
            // those are also the Audience property in each microservice AddAuthentication().JwtBearer 
            return new ApiResource[]
            {
                new ApiResource(SystemConsts.Resources.RosasApi,"Rosas API Resource")
                {
                     Scopes =
                    {
                        SystemConsts.Scopes.Api,
                        SystemConsts.Scopes.SuperAdmin,
                        SystemConsts.Scopes.ClientAdmin,
                        SystemConsts.Scopes.Tenant,
                        SystemConsts.Scopes.ExternalSystem,
                    }
                },
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            //Be aware, that scopes are purely for authorizing clients - not users. 
            //This additional user centric authorization is application logic and not covered by OAuth.
            return new ApiScope[]
           {
                new ApiScope(SystemConsts.Scopes.Api,"rosas API Scope"),
                new ApiScope(SystemConsts.Scopes.SuperAdmin,"Rosas's Super Admin Scope"),
                new ApiScope(SystemConsts.Scopes.ClientAdmin,"Rosas Client's Admin Scope"),
                new ApiScope(SystemConsts.Scopes.Tenant,   "tenant Scope"),
                new ApiScope(SystemConsts.Scopes.ExternalSystem,"External System Scope"),
           };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
                               {
                                    new TestUser
                                    {
                                        SubjectId = "eef27140-b1f0-4b9c-8791-2c49c8382ecc",
                                        Username = "a.aktaa",
                                        IsActive = true,
                                        Password = "P@ssw0rd"
                                    }
                               };
        }

        public static IEnumerable<Client> GetClients(string identityServerUrl)
        {
            var uri = new Uri(identityServerUrl);
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var clients = new[]
            {
                  new Client
                {
                    ClientId = SystemConsts.Clients.AdminPanel,
                    ClientName = "Rosas Admin Panel Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("BSUDEFXZH23JM5N6P0R9WAUCVDWFYGZH3B4M5P7Q8R9TKUCVEXFYG2JK34".Sha256())},
                    RequireClientSecret = true,
                    RequirePkce = false,
                    RedirectUris = { $"{identityServerUrl}/signin-oidc" },
                    FrontChannelLogoutUri = $"{identityServerUrl}/signout-callback-oidc",
                    PostLogoutRedirectUris = { $"{identityServerUrl}/signout-callback-oidc" },
                    AllowedCorsOrigins =     { $"{baseUri}"},
                    AllowedScopes = {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                           SystemConsts.Scopes.Api,
                           SystemConsts.Scopes.SuperAdmin,
                           SystemConsts.Scopes.ClientAdmin,
                           SystemConsts.Scopes.Tenant,
                           SystemConsts.Scopes.ExternalSystem,

                    },
                    AllowPlainTextPkce = true,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    UpdateAccessTokenClaimsOnRefresh =  true,
                    AccessTokenLifetime =  2592000, // 30 days
                    IdentityTokenLifetime = 2592000, // 30 days
                    RefreshTokenExpiration =  TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime =  2592000, // 30 days
                    AbsoluteRefreshTokenLifetime =  0
                },
                  new Client()
                {
                    ClientId = SystemConsts.Clients.OsosExternalSystem,
                    ClientName = "OSOS System",
                    Description = "OSOS is External System of Roaa Tech client. It's calls the ROSAS API to manage its tenants.",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = false,
                    AllowedScopes = { SystemConsts.Scopes.ExternalSystem, },
                    AccessTokenLifetime = 3600, //in seconds = 1hour
                    AccessTokenType = AccessTokenType.Jwt,

                     // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("EHMcQfTjWnZr4u7ADFJaNdRgUkXp2s5v8yBEHKbPeShVmYq3t6wZH3B4M9".Sha256())
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {SystemConsts.Clients.Properties.RosasClientId , SystemConsts.Clients.Properties.Vlaue.RosasClientId},
                        {SystemConsts.Clients.Properties.RosasProductId ,SystemConsts.Clients.Properties.Vlaue.OsosProductId},
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(SystemConsts.Clients.Claims.ClaimType,SystemConsts.Clients.Claims.ExternalSystem)
                    }
                }
                  ,
                  new Client()
                {
                    ClientId = SystemConsts.Clients.ShamsExternalSystem,
                    ClientName = "SHAMS System",
                    Description = "SHAMS is External System of Roaa Tech client. It's calls the ROSAS API to manage its tenants.",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = false,
                    AllowedScopes = { SystemConsts.Scopes.ExternalSystem, },
                    AccessTokenLifetime = 3600, //in seconds = 1hour
                    AccessTokenType = AccessTokenType.Jwt,

                     // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("MNFcPoTjWnZr8u6ADFJaNsAgUfXp4s0v7yBQEZlgWShYxCq3t6wZH3R6X0".Sha256())
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {SystemConsts.Clients.Properties.RosasClientId , SystemConsts.Clients.Properties.Vlaue.RosasClientId},
                        {SystemConsts.Clients.Properties.RosasProductId ,SystemConsts.Clients.Properties.Vlaue.ShamsProductId},
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(SystemConsts.Clients.Claims.ClaimType,SystemConsts.Clients.Claims.ExternalSystem)
                    }
                },
                new Client()
                {
                    ClientId = SystemConsts.Clients.ApptomatorExternalSystem,
                    ClientName = "Apptomator System",
                    Description = "Apptomator is External System of Roaa Tech client. It's calls the ROSAS API to manage its tenants.",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = false,
                    AllowedScopes = { SystemConsts.Scopes.ExternalSystem, },
                    AccessTokenLifetime = 3600, //in seconds = 1hour
                    AccessTokenType = AccessTokenType.Jwt,

                     // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("6etAQfTjWnZr4u7ADFJaNdRgUkX83Tav8yBEHKbPeShVmYq3t6wZH3BETA".Sha256())
                    },
                    Properties = new Dictionary<string, string>
                    {
                        {SystemConsts.Clients.Properties.RosasClientId , SystemConsts.Clients.Properties.Vlaue.RosasClientId},
                        {SystemConsts.Clients.Properties.RosasProductId ,SystemConsts.Clients.Properties.Vlaue.ApptomatorProductId},
                    },
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim(SystemConsts.Clients.Claims.ClaimType,SystemConsts.Clients.Claims.ExternalSystem)
                    }
                },
        };

            return clients;
        }
    }
}

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Infrastructure.Persistence.SeedData.IdentityServer4
{
    public class IdentityServerConfigurationDbInitialiser
    {
        #region Props   
        private readonly IdentityServerConfigurationDbContext _configurationDbContext;
        private readonly IdentityServerPersistedGrantDbContext _persistedGrantDbContext;
        private readonly ILogger<IdentityServerConfigurationDbInitialiser> _logger;
        private readonly IdentityServerOptions _identityServerSettings;
        #endregion

        #region Ctors 
        public IdentityServerConfigurationDbInitialiser(IdentityServerConfigurationDbContext configurationDbContex,
                                                        IdentityServerPersistedGrantDbContext persistedGrantDbContext,
                                                        IApiConfigurationService<IdentityServerOptions> identityServerSettings,
                                                        ILogger<IdentityServerConfigurationDbInitialiser> logger)
        {
            _configurationDbContext = configurationDbContex;
            _persistedGrantDbContext = persistedGrantDbContext;
            _identityServerSettings = identityServerSettings.Options;
            _logger = logger;
        }
        #endregion


        #region Services   
        public async Task MigrateAsync()
        {
            if (_identityServerSettings.MigrateDatabase)
            {
                try
                {
                    await _configurationDbContext.Database.MigrateAsync();
                    await _persistedGrantDbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initialising the identity server configuration database.");
                    throw;
                }
            }
        }

        public async Task SeedAsync()
        {
            if (_identityServerSettings.SeedData)
            {
                try
                {
                    await TrySeedAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding the database.");
                    throw;
                }
            }
        }

        private async Task TrySeedAsync()
        {
            var clients = IdentityServer4Config.GetClients(_identityServerSettings.Url);
            if (clients != null)
            {
                foreach (var client in clients)
                {
                    if (!_configurationDbContext.Clients.Any(c => c.ClientId == client.ClientId))
                    {
                        _configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    else
                    {
                        var existingClient = _configurationDbContext.Clients.Include(x => x.AllowedScopes).FirstOrDefault(c => c.ClientId == client.ClientId);

                        foreach (var scope in client.AllowedScopes)
                        {
                            if (!existingClient.AllowedScopes.Any(s => s.ClientId == existingClient.Id && s.Scope == scope))
                            {
                                existingClient.AllowedScopes.Add(new ClientScope { Scope = scope, ClientId = existingClient.Id });
                            }
                        }
                    }
                }
                await _configurationDbContext.SaveChangesAsync();
            }

            if (IdentityServer4Config.GetIdentityResources() != null)
            {
                foreach (var resource in IdentityServer4Config.GetIdentityResources())
                {
                    if (!_configurationDbContext.IdentityResources.Any(r => r.Name == resource.Name))
                        _configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
                await _configurationDbContext.SaveChangesAsync();
            }

            if (IdentityServer4Config.GetApiScopes() != null)
            {
                foreach (var resource in IdentityServer4Config.GetApiScopes())
                {
                    if (!_configurationDbContext.ApiScopes.Any(r => r.Name == resource.Name))
                        _configurationDbContext.ApiScopes.Add(resource.ToEntity());
                }
                await _configurationDbContext.SaveChangesAsync();
            }

            if (IdentityServer4Config.GetApiResources() != null)
            {
                foreach (var resource in IdentityServer4Config.GetApiResources())
                {
                    var apiResource = await _configurationDbContext.ApiResources.Where(r => r.Name == resource.Name).FirstOrDefaultAsync();

                    if (apiResource is null)
                    {
                        _configurationDbContext.ApiResources.Add(resource.ToEntity());
                    }
                    else
                    {
                        var scopes = await _configurationDbContext.ApiResourceScopes.Where(x => x.ApiResourceId == apiResource.Id).ToListAsync();

                        foreach (var scope in resource.Scopes)
                        {
                            if (!scopes.Where(x => x.Scope.Equals(scope, StringComparison.OrdinalIgnoreCase)).Any())
                            {
                                apiResource.Scopes.Add(new ApiResourceScope { Scope = scope, ApiResourceId = apiResource.Id });
                            }
                        }
                    }
                }
                await _configurationDbContext.SaveChangesAsync();
            }

        }

        #endregion
    }
}



﻿using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using MediatR;
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
        private readonly IPublisher _publisher;
        private readonly ClientStore _clientStore;
        #endregion

        #region Ctors 
        public IdentityServerConfigurationDbInitialiser(IdentityServerConfigurationDbContext configurationDbContex,
                                                        IdentityServerPersistedGrantDbContext persistedGrantDbContext,
                                                        IApiConfigurationService<IdentityServerOptions> identityServerSettings,
                                                        ILogger<IdentityServerConfigurationDbInitialiser> logger,
                                                        ClientStore clientStore,
                                                        IPublisher publisher)
        {
            _configurationDbContext = configurationDbContex;
            _persistedGrantDbContext = persistedGrantDbContext;
            _identityServerSettings = identityServerSettings.Options;
            _clientStore = clientStore;
            _publisher = publisher;
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
                    await FixAsync();
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
                    var clientInDb = await _configurationDbContext.Clients
                                                            .Include(x => x.AllowedScopes)
                                                            .Include(x => x.Properties)
                                                            .SingleOrDefaultAsync(c => c.ClientId == client.ClientId);
                    if (clientInDb is null)
                    {
                        _configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    else
                    {
                        foreach (var scope in client.AllowedScopes)
                        {
                            if (!clientInDb.AllowedScopes.Any(s => s.ClientId == clientInDb.Id && s.Scope == scope))
                            {
                                clientInDb.AllowedScopes.Add(new ClientScope { Scope = scope, ClientId = clientInDb.Id });
                            }
                        }

                        foreach (var property in client.Properties)
                        {
                            if (!clientInDb.Properties.Any(p => p.ClientId == clientInDb.Id && p.Key == property.Key))
                            {
                                clientInDb.Properties.Add(new ClientProperty
                                {
                                    ClientId = clientInDb.Id,
                                    Key = property.Key,
                                    Value = property.Value,
                                });
                            }
                        }
                    }
                }
                await _configurationDbContext.SaveChangesAsync();



                foreach (var client in clients)
                {
                    if (client.Properties is not null && client.Properties.Any())
                    {
                        if (client.Properties.TryGetValue(SystemConsts.Clients.Properties.RosasProductId, out string? productId) &&
                            client.Properties.TryGetValue(SystemConsts.Clients.Properties.RosasClientId, out string? clientId) &&
                            client.Properties.TryGetValue(SystemConsts.Clients.Properties.RosasUserId, out string? userId)
                            )
                        {
                            var id = await _configurationDbContext.Clients
                                                            .Where(c => c.ClientId == client.ClientId)
                                                            .Select(x => x.Id)
                                                            .SingleOrDefaultAsync();

                            if (!await _configurationDbContext.ClientCustomDetails.Where(x => x.ClientId == id).AnyAsync())

                                _configurationDbContext.ClientCustomDetails.Add(new Domain.Entities.ClientCustomDetail
                                {
                                    ClientId = id,
                                    ProductId = new Guid(productId),
                                    UserId = new Guid(userId),
                                    ProductOwnerClientId = new Guid(clientId),

                                });
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

        private async Task FixAsync()
        {
            await Remove("roaa-apptomator");
            await Remove("roaa-shams");
            await Remove("roaa-osos");


            var clientsCustomDetailsInDb = await _configurationDbContext.ClientCustomDetails
                                                      .Where(x => x.ClientType == Domain.Entities.ClientType.None)
                                                      .ToListAsync();


            foreach (var c in clientsCustomDetailsInDb)
            {
                c.ClientType = Domain.Entities.ClientType.ExternalSystem;
            }

            await _configurationDbContext.SaveChangesAsync();
        }
        private async Task Remove(string clientId)
        {
            IQueryable<Client> baseQuery = _configurationDbContext.Clients
                                          .Where(x => x.ClientId == clientId);

            var client = (await baseQuery.ToArrayAsync())
                            .SingleOrDefault(x => x.ClientId == clientId);

            if (client is not null && client.Created.Year < 2024)
            {
                await baseQuery.Include(x => x.AllowedCorsOrigins).SelectMany(c => c.AllowedCorsOrigins).LoadAsync();
                await baseQuery.Include(x => x.AllowedGrantTypes).SelectMany(c => c.AllowedGrantTypes).LoadAsync();
                await baseQuery.Include(x => x.AllowedScopes).SelectMany(c => c.AllowedScopes).LoadAsync();
                await baseQuery.Include(x => x.Claims).SelectMany(c => c.Claims).LoadAsync();
                await baseQuery.Include(x => x.ClientSecrets).SelectMany(c => c.ClientSecrets).LoadAsync();
                await baseQuery.Include(x => x.IdentityProviderRestrictions).SelectMany(c => c.IdentityProviderRestrictions).LoadAsync();
                await baseQuery.Include(x => x.PostLogoutRedirectUris).SelectMany(c => c.PostLogoutRedirectUris).LoadAsync();
                await baseQuery.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();
                await baseQuery.Include(x => x.RedirectUris).SelectMany(c => c.RedirectUris).LoadAsync();

                _configurationDbContext.Clients.Remove(client);

                var res = await _configurationDbContext.SaveChangesAsync();
            }



        }
        #endregion
    }
}



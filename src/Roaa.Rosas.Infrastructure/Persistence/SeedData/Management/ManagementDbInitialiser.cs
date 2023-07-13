﻿
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Infrastructure.Persistence.SeedData.Management
{
    public class ManagementDbInitialiser
    {
        #region Props   
        private readonly IRosasDbContext _dbContext;
        private readonly ILogger<ManagementDbInitialiser> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly GeneralOptions _settings;
        #endregion

        #region Ctors     
        public ManagementDbInitialiser(IRosasDbContext identityDbContext,
                                     IWebHostEnvironment environmen,
                                     IApiConfigurationService<GeneralOptions> settings,
                                     ILogger<ManagementDbInitialiser> logger)
        {
            _logger = logger;
            _environment = environmen;
            _settings = settings.Options;
            _dbContext = identityDbContext;
        }
        #endregion


        #region Services  
        public async Task MigrateAsync()
        {
            if (_settings.MigrateDatabase)
            {
                try
                {
                    await _dbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initialising the management database.");
                    throw;
                }
            }
        }

        public async Task SeedAsync()
        {
            if (_settings.SeedData)
            {
                try
                {
                    await TrySeedClientsAsync();
                    await TrySeedProductsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding the management database.");
                    throw;
                }
            }
        }



        private async Task TrySeedClientsAsync()
        {
            foreach (var client in GetClients())
            {
                if (!_dbContext.Clients.Any(x => x.Id == client.Id))
                {
                    _dbContext.Clients.Add(client);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task TrySeedProductsAsync()
        {
            foreach (var product in GetProducts())
            {
                if (!_dbContext.Products.Any(x => x.Id == product.Id))
                {
                    _dbContext.Products.Add(product);
                }
            }

            await _dbContext.SaveChangesAsync();
        }


        #endregion 

        private List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                    {
                        Id = new Guid("88283b02-e969-485a-a5a3-9e5d1d0d3337"),
                        UniqueName = "roaa",
                        Title= "Roaa Tech",
                        Created = DateTime.Now,
                        Edited = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        EditedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted = false,
                    },
            };
        }

        private List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.OsosProductId),
                        ClientId = new Guid("88283b02-e969-485a-a5a3-9e5d1d0d3337"),
                        UniqueName = "osos",
                        Title = "OSOS",
                        Url = "https://osos.roaa.tech",
                        Created = DateTime.Now,
                        Edited = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        EditedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted= false,
                    },
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.ShamsProductId),
                        ClientId = new Guid("88283b02-e969-485a-a5a3-9e5d1d0d3337"),
                        UniqueName = "shams",
                        Title = "SHAMS",
                        Url = "https://shams.roaa.tech",
                        Created = DateTime.Now,
                        Edited = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        EditedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted= false,
                    },
            };
        }
    }
}

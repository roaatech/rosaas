
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Infrastructure.Persistence.SeedData.Management
{
    public class ManagementDbInitialiser
    {
        #region Props   
        private readonly IRosasDbContext _dbContext;
        private readonly ILogger<ManagementDbInitialiser> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly GeneralOptions _settings;
        private readonly IdentityServerOptions _identityServerOptions;
        private readonly ISettingService _settingService;
        private string _baseUrl;
        #endregion

        #region Ctors     
        public ManagementDbInitialiser(IRosasDbContext identityDbContext,
                                     IWebHostEnvironment environmen,
                                     IApiConfigurationService<GeneralOptions> settings,
                                     IApiConfigurationService<IdentityServerOptions> identityServerOptions,
                                     ISettingService settingService,
                                     ILogger<ManagementDbInitialiser> logger)
        {
            _logger = logger;
            _environment = environmen;
            _settings = settings.Options;
            _identityServerOptions = identityServerOptions.Options;
            _dbContext = identityDbContext;
            _settingService = settingService;
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
                    await FixFeaturesResetAsync();
                    await TrySeedClientsAsync();
                    await TrySeedProductsAsync();

                    await _settingService.SaveSettingAsync(new ProductWarningsSettings());
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
                var productInDb = await _dbContext.Products.Where(x => x.Id == product.Id).SingleOrDefaultAsync();

                if (productInDb is null)
                {
                    _dbContext.Products.Add(product);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(productInDb.DisplayName))
                    {
                        productInDb.DisplayName = productInDb.Name;
                    }

                }
            }

            await _dbContext.SaveChangesAsync();
        }


        #endregion
        private async Task FixFeaturesResetAsync()
        {
            var plansFeatures = await _dbContext.PlanFeatures
                                               .Include(x => x.Feature)
                                               .ToListAsync();

            const string key = "SeedData.Management.ManagementDbInitialiser.FeaturesResetFixed";

            if (!await _dbContext.Settings
                                .Where(x => x.Key.Equals(key))
                                .AnyAsync())
            {
                foreach (var pf in plansFeatures)
                {
                    pf.FeatureReset = pf.Feature.FeatureReset;
                }

                _dbContext.Settings.Add(new Setting
                {
                    Key = key,
                    Value = DateTime.UtcNow.ToString(),
                    Id = Guid.NewGuid()
                });

                await _dbContext.SaveChangesAsync();
            }
        }
        private List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                    {
                        Id = new Guid("88283b02-e969-485a-a5a3-9e5d1d0d3337"),
                        Name = "roaa",
                        DisplayName= "Roaa Tech",
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        ModifiedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
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
                        ClientId =  new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        Name = "OSOS",
                        DisplayName = "OSOS",
                        DefaultHealthCheckUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/{{name}}/health-check",
                        HealthStatusInformerUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/health-status-unhealthy",
                        CreationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/cerated",
                        ActivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/active",
                        DeactivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/inactive",
                        DeletionUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/deleted",
                        SubscriptionResetUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/reset",
                        SubscriptionUpgradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/upgrade",
                        SubscriptionDowngradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/downgrade",
                        ApiKey = "4s0v7yBQEZShYxCq3tlsAgUfXpgW",
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        ModifiedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted= false,
                    },
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.ShamsProductId),
                        ClientId =  new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        Name = "SHAMS",
                        DisplayName = "SHAMS",
                        DefaultHealthCheckUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/{{name}}/health-check",
                        HealthStatusInformerUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/health-status-unhealthy",
                        CreationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/cerated",
                        ActivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/active",
                        DeactivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/inactive",
                        DeletionUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/deleted",
                        SubscriptionResetUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/reset",
                        SubscriptionUpgradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/upgrade",
                        SubscriptionDowngradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/downgrade",
                        ApiKey = "4s0v7yBQEZShYxCq3tlsAgUfXpgW",
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        ModifiedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted= false,
                    },
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.ApptomatorProductId),
                        ClientId =  new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        Name = "Apptomator",
                        DisplayName = "Apptomator",
                        DefaultHealthCheckUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/{{name}}/health-check",
                        HealthStatusInformerUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/health-status-unhealthy",
                        CreationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/cerated",
                        ActivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/active",
                        DeactivationUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/inactive",
                        DeletionUrl = $"{_identityServerOptions.Url}/external-system-simulator/tenants/deleted",
                        SubscriptionResetUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/reset",
                        SubscriptionUpgradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/upgrade",
                        SubscriptionDowngradeUrl  = $"{_identityServerOptions.Url}/external-system-simulator/tenants/downgrade",
                        ApiKey = "4s0v7yBQEZShYxCq3tlsAgUfXpgW",
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                        CreatedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        ModifiedByUserId = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                        IsDeleted= false,
                    },
            };
        }
    }
}


using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
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
                    //await FixOrderPaymentStatusAsync();
                    //await FixPlanPriceNameAsync();
                    //await FixFeaturesResetAsync();
                    //await FixPlanTenancyTypeAsync();
                    await FixTenanatRequestAsync();
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
            const string key = "SeedData.Management.ManagementDbInitialiser.DefaultClientAndUserAndPlanAndPriceSeeded2";
            bool anyKey = await _dbContext.Settings
                                      .Where(x => x.Key.Equals(key))
                                      .AnyAsync();
            bool settingSeeded = false;

            foreach (var product in GetProducts())
            {
                var productInDb = await _dbContext.Products.Where(x => x.Id == product.Id).SingleOrDefaultAsync();

                if (productInDb is null)
                {

                    product.AddDomainEvent(new ProductCreatedEvent(product, new Guid(SystemConsts.Users.RosaasSystem), UserType.RosasSystem));
                    _dbContext.Products.Add(product);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(productInDb.DisplayName))
                    {
                        productInDb.DisplayName = productInDb.SystemName;
                    }

                    if (!anyKey)
                    {
                        if (!await _dbContext.Plans.Where(x => x.ProductId == product.Id &&
                                                          x.TenancyType == Domain.Enums.TenancyType.Unlimited)
                                            .AnyAsync())
                        {
                            productInDb.ModificationDate = DateTime.UtcNow;
                            productInDb.ModifiedByUserId = new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId);
                            productInDb.AddDomainEvent(new ProductCreatedEvent(productInDb, new Guid(SystemConsts.Users.RosaasSystem), UserType.RosasSystem));
                        }
                        if (!settingSeeded)
                        {
                            _dbContext.Settings.Add(new Setting
                            {
                                Key = key,
                                Value = DateTime.UtcNow.ToString(),
                                Id = Guid.NewGuid()
                            });
                            settingSeeded = true;
                        }

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
        private async Task FixPlanPriceNameAsync()
        {
            var planPrices = await _dbContext.PlanPrices
                                               .Include(x => x.Plan)
                                               .ToListAsync();

            const string key = "SeedData.Management.ManagementDbInitialiser.PlanPriceNameFixed";

            if (!await _dbContext.Settings
                                .Where(x => x.Key.Equals(key))
                                .AnyAsync())
            {
                foreach (var pp in planPrices)
                {
                    if (string.IsNullOrWhiteSpace(pp.SystemName))
                    {
                        pp.SystemName = $"{pp.Plan.SystemName}-{(int)pp.Price}-{pp.PlanCycle.ToString()}".ToLower();

                    }
                }

                _dbContext.Settings.Add(new Setting
                {
                    Key = key,
                    Value = DateTime.UtcNow.ToString(),
                    Id = Guid.NewGuid()
                });

                await _dbContext.SaveChangesAsync();
            }


            const string key2 = "SeedData.Management.ManagementDbInitialiser.PlanPriceUnlimitedCycleFixed";

            if (!await _dbContext.Settings
                                .Where(x => x.Key.Equals(key2))
                                .AnyAsync())
            {
                foreach (var pp in planPrices)
                {
                    if (pp.Plan.TenancyType == Domain.Enums.TenancyType.Unlimited)
                    {
                        pp.PlanCycle = PlanCycle.Unlimited;
                        pp.SystemName = pp.SystemName.Replace("unlimited-custom", $"open-{PlanCycle.Unlimited}".ToLower());
                    }
                }

                _dbContext.Settings.Add(new Setting
                {
                    Key = key2,
                    Value = DateTime.UtcNow.ToString(),
                    Id = Guid.NewGuid()
                });

                await _dbContext.SaveChangesAsync();
            }
        }
        private async Task FixPlanTenancyTypeAsync()
        {

            const string key = "SeedData.Management.ManagementDbInitialiser.PlanTenancyTypeFixed";

            if (!await _dbContext.Settings
                                .Where(x => x.Key.Equals(key))
                                .AnyAsync())
            {

                var plans = await _dbContext.Plans.ToListAsync();


                foreach (var plan in plans)
                {
                    if ((int)plan.TenancyType == 0)
                    {
                        plan.TenancyType = Domain.Enums.TenancyType.Planed;
                    }
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
        private async Task FixOrderPaymentStatusAsync()
        {

            const string key = "SeedData.Management.ManagementDbInitialiser.OrderPaymentStatusFixed";

            if (!await _dbContext.Settings
                                .Where(x => x.Key.Equals(key))
                                .AnyAsync())
            {

                var orders = await _dbContext.Orders.Where(x => x.PaymentStatus == null).ToListAsync();


                foreach (var order in orders)
                {

                    order.PaymentStatus = PaymentStatus.None;
                }
            }

            _dbContext.Settings.Add(new Setting
            {
                Key = key,
                Value = DateTime.UtcNow.ToString(),
                Id = Guid.NewGuid()
            });

            await _dbContext.SaveChangesAsync();
        }
        private List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        SystemName = "roaa",
                        DisplayName= "Roaa Tech",
                        CreationDate = DateTime.Now,
                        ModificationDate = DateTime.Now,
                        CreatedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        ModifiedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
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
                        SystemName = "OSOS",
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
                        CreatedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        ModifiedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        IsDeleted= false,
                    },
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.ShamsProductId),
                        ClientId =  new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        SystemName = "SHAMS",
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
                        CreatedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        ModifiedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        IsDeleted= false,
                    },
                new Product
                    {
                        Id = new Guid(SystemConsts.Clients.Properties.Vlaue.ApptomatorProductId),
                        ClientId =  new Guid(SystemConsts.Clients.Properties.Vlaue.RosasClientId),
                        SystemName = "Apptomator",
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
                        CreatedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        ModifiedByUserId = new Guid(SystemConsts.Users.RosaasSystem),
                        IsDeleted= false,
                    },
            };
        }


        private async Task FixTenanatRequestAsync()
        {

            const string key = "SeedData.Management.ManagementDbInitialiser.TenantCreationRequestFixed";
            if (!await _dbContext.Settings
                               .Where(x => x.Key.Equals(key))
                               .AnyAsync())
            {
                var requests = await _dbContext.TenantCreationRequests.ToListAsync();
                var orders = await _dbContext.OrderItems.Select(x => new { x.OrderId, x.ProductId }).ToListAsync();
                foreach (var request in requests)
                {
                    if (orders.Where(x => x.OrderId == request.OrderId).Any())
                        request.ProductIds = orders.Where(x => x.OrderId == request.OrderId).Select(x => x.ProductId).ToList();
                }
            }

            _dbContext.Settings.Add(new Setting
            {
                Key = key,
                Value = DateTime.UtcNow.ToString(),
                Id = Guid.NewGuid()
            });

            await _dbContext.SaveChangesAsync();
        }
        private async Task FixSubscriptionPlanChangeStatusAsync()
        {

            const string key = "SeedData.Management.ManagementDbInitialiser.SubscriptionPlanChangeStatusFixed";
            if (!await _dbContext.Settings
                               .Where(x => x.Key.Equals(key))
                               .AnyAsync())
            {
                // SubscriptionPlanChangeStatus
                var subscriptions = await _dbContext.Subscriptions.Where(x => x.SubscriptionPlanChangeStatus == null).ToListAsync();
                foreach (var subscription in subscriptions)
                {
                    subscription.SubscriptionPlanChangeStatus = SubscriptionPlanChangeStatus.None;
                }
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
}

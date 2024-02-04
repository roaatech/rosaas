using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Infrastructure.Persistence.SeedData.Identity
{
    public class IdentityDbInitialiser
    {
        #region Props   
        private readonly IRosasDbContext _identityDbContext;
        private readonly ILogger<IdentityDbInitialiser> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly GeneralOptions _settings;
        #endregion

        #region Ctors     
        public IdentityDbInitialiser(IRosasDbContext identityDbContext,
                                     IWebHostEnvironment environmen,
                                     IApiConfigurationService<GeneralOptions> settings,
                                     ILogger<IdentityDbInitialiser> logger)
        {
            _logger = logger;
            _environment = environmen;
            _settings = settings.Options;
            _identityDbContext = identityDbContext;
        }
        #endregion


        #region Services  
        public async Task MigrateAsync()
        {
            if (_settings.MigrateDatabase)
            {
                try
                {
                    await _identityDbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initialising the identity database.");
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
                    await TrySeedAsync();
                    await TrySeedSuperAdminsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding the identity database.");
                    throw;
                }
            }
        }

        private async Task TrySeedAsync()
        {
            #region Dummy Data  

            if (!_environment.IsProductionEnvironment() && _settings.CreateDummyData)
            {

            }
            #endregion

            await _identityDbContext.SaveChangesAsync();
        }

        private async Task TrySeedSuperAdminsAsync()
        {
            foreach (var user in GetSuperAdmins())
            {
                if (!_identityDbContext.Users.Any(c => c.Id == user.Id))
                {
                    _identityDbContext.Users.Add(user);
                }
            }

            await _identityDbContext.SaveChangesAsync();
        }


        #endregion


        private List<User> GetSuperAdmins()
        {
            var admin1 = new User
            {
                Id = new Guid("9728990f-841c-45bd-b358-14b308c80030"),
                UserName = "roaa.admin",
                NormalizedUserName = "roaa.admin".ToUpper(),
                Email = "admin@roaa.tech",
                NormalizedEmail = "admin@roaa.tech".ToUpper(),
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                EmailConfirmed = true,
                IsActive = true,
                Locale = "en",
                Status = UserStatus.Ready,
                UserType = UserType.SuperAdmin,
            };

            var admin2 = new User
            {
                Id = new Guid("6640db77-5436-40ee-982c-0ee8bdb151aa"),
                UserName = "a.aktaa",
                NormalizedUserName = "a.aktaa".ToUpper(),
                Email = "ahmad.aktaa@roaa.com",
                NormalizedEmail = "ahmad.aktaa@roaa.com".ToUpper(),
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                EmailConfirmed = true,
                IsActive = true,
                Locale = "en",
                Status = UserStatus.Ready,
                UserType = UserType.SuperAdmin,
            };



            var admin3 = new User
            {
                Id = new Guid(SystemConsts.Users.RosaasSystem),
                UserName = "system.admin",
                NormalizedUserName = "system.admin".ToUpper(),
                Email = "system.admin@roaa.com",
                NormalizedEmail = "system.admin@roaa.com".ToUpper(),
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                EmailConfirmed = true,
                IsActive = true,
                Locale = "en",
                Status = UserStatus.Ready,
                UserType = UserType.RosasSystem,
            };


            // create a new password hasher
            var hasher = new PasswordHasher<User>();

            // hash the password and set it for the user
            var hashedPassword1 = hasher.HashPassword(admin1, "U3hsvI1Kc#J0$U3d7@RH");
            var hashedPassword2 = hasher.HashPassword(admin2, "E8$r13z^u6Eav#^4OAaP");
            var hashedPassword3 = hasher.HashPassword(admin3, "E8$r13z^u6Eav#^4OAaP");

            admin1.PasswordHash = hashedPassword1;
            admin2.PasswordHash = hashedPassword2;
            admin3.PasswordHash = hashedPassword3;

            return new List<User> { admin1, admin2, admin3 };
        }
    }
}

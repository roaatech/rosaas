using Microsoft.AspNetCore.Identity;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Infrastructure.Persistence.DbContexts;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class IdentityConfigurations
    {
        public static void AddIdentityConfigurations(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          IWebHostEnvironment environment,
                                                          RootOptions rootOptions)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<RosasIdentityDbContext>()
            .AddDefaultTokenProviders();
        }


    }
}





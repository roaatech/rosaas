using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Infrastructure.Persistence.DbContexts;

namespace Roaa.Rosas.Framework.Configurations
{
    public static class DbContextConfigurations
    {
        public static void AddDbContextConfigurations(this IServiceCollection services,
                                                           IConfiguration configuration,
                                                           IWebHostEnvironment environment,
                                                           RootOptions rootOptions)
        {
            services.AddDbContext<RosasDbContext>(options =>
                options.UseMySql(rootOptions.ConnectionStrings.IdentityDb, new MySqlServerVersion(new Version())));
        }


    }
}





using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTableName("IdentityRoleClaims");
        }
        #endregion
    }
    public static class dddddsd
    {
        public static EntityTypeBuilder ToTableName(this EntityTypeBuilder builder, string? name)
        {
            return builder.ToTable(name.ToTableNamingStrategy());
        }

        public static string ToTableNamingStrategy(this string name)
        {
            return name.ToSnakeCaseNamingStrategy();
        }
    }
}

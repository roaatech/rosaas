using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("Identity_RoleClaims");
        }
        #endregion
    }
}

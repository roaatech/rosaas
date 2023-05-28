using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<Role>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Identity_Roles");
        }
        #endregion
    }
}

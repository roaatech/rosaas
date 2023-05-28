using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("Identity_UserRoles");
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTableName("IdentityUserRoles");
        }
        #endregion
    }
}

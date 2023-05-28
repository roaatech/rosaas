using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("Identity_UserLogins");
        }
        #endregion
    }
}

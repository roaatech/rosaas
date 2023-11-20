using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTableName("IdentityUserLogins");
        }
        #endregion
    }
}

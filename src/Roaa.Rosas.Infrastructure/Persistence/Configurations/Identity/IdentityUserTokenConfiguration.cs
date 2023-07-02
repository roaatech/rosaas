using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTableName("IdentityUserTokens");
        }
        #endregion
    }
}

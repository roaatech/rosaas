using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTableName("IdentityUserClaims");
        }
        #endregion
    }
}

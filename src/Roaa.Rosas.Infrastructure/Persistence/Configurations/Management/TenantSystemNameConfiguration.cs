using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantSystemNameConfiguration : IEntityTypeConfiguration<TenantSystemName>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantSystemName> builder)
        {
            builder.ToTableName("RosasTenantSystemNames");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantNormalizedSystemName).IsRequired().HasMaxLength(250);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

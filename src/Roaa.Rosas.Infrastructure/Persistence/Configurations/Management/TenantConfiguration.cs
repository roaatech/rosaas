using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTableName("RosasTenants");
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.SystemName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantCreationRequestConfiguration : IEntityTypeConfiguration<TenantCreationRequest>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantCreationRequest> builder)
        {
            builder.ToTableName("RosasTenantCreationRequests");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(250);
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.SubtotalInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.SubtotalExclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.Total).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

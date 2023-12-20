using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantCreationRequestItemConfiguration : BaseEntityConfiguration<Specification>, IEntityTypeConfiguration<TenantCreationRequestItem>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantCreationRequestItem> builder)
        {
            builder.ToTableName("RosasTenantCreationRequestItems");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.SystemName).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.UnitPriceInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.UnitPriceExclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.PriceInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.PriceExclTax).HasPrecision(8, 2).IsRequired();
            builder.HasOne(b => b.TenantCreationRequest).WithMany(p => p.Items).HasForeignKey(f => f.TenantCreationRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.Specifications)
                   .IsUnicode()
                   .HasConversion(
                           ConvertLocalizedStringToJson<List<TenantCreationRequestItemSpecification>>(),
                           ConvertJsonToLocalizedString<List<TenantCreationRequestItemSpecification>>()
                    );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

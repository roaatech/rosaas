using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class OrderItemConfiguration : BaseEntityConfiguration<Specification>, IEntityTypeConfiguration<OrderItem>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTableName("RosasOrderItems");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.SystemName).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.UnitPriceInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.UnitPriceExclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.PriceInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.PriceExclTax).HasPrecision(8, 2).IsRequired();
            builder.HasOne(b => b.Order).WithMany(p => p.OrderItems).HasForeignKey(f => f.OrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Subscription).WithMany(p => p.OrderItems).HasForeignKey(f => f.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.Specifications)
                   .IsUnicode()
                   .HasConversion(
                           ConvertLocalizedStringToJson<List<OrderItemSpecification>>(),
                           ConvertJsonToLocalizedString<List<OrderItemSpecification>>()
                    );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

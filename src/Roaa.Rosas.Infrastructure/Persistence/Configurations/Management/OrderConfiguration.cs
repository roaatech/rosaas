using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTableName("RosasOrders");
            builder.HasKey(x => x.Id);
            builder.HasAlternateKey(r => r.OrderNumber);
            builder.Property(r => r.OrderNumber).ValueGeneratedOnAdd();
            builder.Property(r => r.UserCurrencyCode).IsRequired(true).HasMaxLength(3).IsUnicode();
            builder.Property(r => r.AuthorizationTransactionId).IsRequired(false).HasMaxLength(250).IsUnicode();
            builder.Property(r => r.AuthorizationTransactionCode).IsRequired(false).HasMaxLength(250).IsUnicode();
            builder.Property(r => r.Reference).IsRequired(false).HasMaxLength(250).IsUnicode();
            builder.Property(r => r.CurrencyRate).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.OrderSubtotalInclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.OrderSubtotalExclTax).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.OrderTotal).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Tenant).WithMany(p => p.Orders).HasForeignKey(f => f.TenantId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionCycleConfiguration : IEntityTypeConfiguration<SubscriptionCycle>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionCycle> builder)
        {
            builder.ToTableName("RosasSubscriptionCycles");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.PlanName).IsRequired().HasMaxLength(250).IsUnicode();
            builder.HasOne(b => b.Subscription).WithMany(p => p.SubscriptionCycles).HasForeignKey(f => f.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.Cycle).IsRequired();
            builder.Property(r => r.Price).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.PlanId).IsRequired();
            builder.Property(r => r.PlanPriceId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.TenantId).IsRequired();

            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.StartDate).IsRequired();
            builder.Property(r => r.EndDate).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

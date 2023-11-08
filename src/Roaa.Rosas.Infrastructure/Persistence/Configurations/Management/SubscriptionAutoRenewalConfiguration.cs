using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionAutoRenewalConfiguration : IEntityTypeConfiguration<SubscriptionAutoRenewal>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionAutoRenewal> builder)
        {
            builder.ToTableName("RosasSubscriptionAutoRenewals");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.Plan).WithMany(p => p.SubscriptionAutoRenewals).HasForeignKey(f => f.PlanId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.PlanPrice).WithMany(p => p.SubscriptionAutoRenewals).HasForeignKey(f => f.PlanPriceId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.PlanId).IsRequired();
            builder.Property(r => r.PlanPriceId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.Cycle).IsRequired();
            builder.Property(r => r.Price).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.CreatedByUserId).IsRequired(true);
            builder.Property(r => r.ModifiedByUserId).IsRequired(true);
            builder.Property(r => r.CreationDate).IsRequired(true);
            builder.Property(r => r.ModificationDate).IsRequired(true);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionFeatureConfiguration : IEntityTypeConfiguration<SubscriptionFeature>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionFeature> builder)
        {
            builder.ToTableName("RosasSubscriptionFeatures");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.PlanFeature).WithMany(p => p.SubscriptionFeatures).HasForeignKey(f => f.PlanFeatureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Feature).WithMany(p => p.SubscriptionFeatures).HasForeignKey(f => f.FeatureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Subscription).WithMany(p => p.SubscriptionFeatures).HasForeignKey(f => f.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.EndDate).IsRequired(false);
            builder.Property(r => r.StartDate).IsRequired(false);
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

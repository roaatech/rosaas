﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionFeatureCycleConfiguration : IEntityTypeConfiguration<SubscriptionFeatureCycle>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionFeatureCycle> builder)
        {
            builder.ToTableName("RosasSubscriptionFeatureCycles");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.SubscriptionFeature).WithMany(p => p.SubscriptionFeatureCycles).HasForeignKey(f => f.SubscriptionFeatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.FeatureName).IsRequired().HasMaxLength(250).IsUnicode();
            builder.Property(r => r.SubscriptionCycleId).IsRequired();
            builder.Property(r => r.SubscriptionFeatureId).IsRequired();
            builder.Property(r => r.PlanFeatureId).IsRequired();
            builder.Property(r => r.FeatureId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.Type).IsRequired();
            builder.Property(r => r.Reset).IsRequired();
            builder.Property(r => r.Cycle).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.StartDate).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

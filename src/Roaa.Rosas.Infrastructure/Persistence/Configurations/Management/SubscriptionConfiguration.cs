﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTableName("RosasSubscriptions");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.PlanPrice).WithMany(p => p.Subscriptions).HasForeignKey(f => f.PlanPriceId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Plan).WithMany(p => p.Subscriptions).HasForeignKey(f => f.PlanId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Tenant).WithMany(p => p.Subscriptions).HasForeignKey(f => f.TenantId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Product).WithMany(p => p.Subscriptions).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.HealthCheckStatus).WithOne(p => p.Subscription).HasForeignKey<TenantHealthStatus>(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.HealthCheckUrl).IsRequired(true).HasMaxLength(250);
            builder.Property(r => r.HealthCheckUrlIsOverridden).IsRequired(true);
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
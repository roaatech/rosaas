﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTableName("RosasProducts");
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.SystemName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.DisplayName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.DefaultHealthCheckUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.HealthStatusInformerUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreationUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.ActivationUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.DeactivationUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.DeletionUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.ApiKey).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.SubscriptionResetUrl).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Client).WithMany(p => p.Products).HasForeignKey(f => f.ClientId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

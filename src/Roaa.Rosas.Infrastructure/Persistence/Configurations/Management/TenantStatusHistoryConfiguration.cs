﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantStatusHistoryConfiguration : IEntityTypeConfiguration<TenantStatusHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantStatusHistory> builder)
        {
            builder.ToTableName("RosasTenantStatusHistory");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.PreviousStatus).IsRequired();
            builder.Property(r => r.Step).IsRequired();
            builder.Property(r => r.PreviousStep).IsRequired();
            builder.Property(r => r.ExpectedResourceStatus).IsRequired();
            builder.Property(r => r.OwnerId).IsRequired();
            builder.Property(r => r.OwnerType).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.TimeStamp).IsRequired();
            builder.Property(r => r.TimeStamp).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Property(r => r.Message).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

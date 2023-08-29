﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantProcessHistoryConfiguration : IEntityTypeConfiguration<TenantProcessHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantProcessHistory> builder)
        {
            builder.ToTableName("RosasTenantProcessHistory");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.ProcessType).IsRequired();
            builder.Property(r => r.OwnerType).IsRequired();
            builder.Property(r => r.Enabled).IsRequired();
            builder.Property(r => r.ProcessDate).IsRequired();
            builder.Property(r => r.TimeStamp).IsRequired().HasConversion(
               v => v.Ticks,
               v => new DateTime(v)
           );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantCreationRequestConfiguration : BaseEntityConfiguration<Order>, IEntityTypeConfiguration<TenantCreationRequest>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantCreationRequest> builder)
        {
            builder.ToTableName("RosasTenantCreationRequests");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.NormalizedSystemName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Property(r => r.ProductIds)
                 .IsRequired(false)
                 .HasMaxLength(500)
                 .HasConversion(
                         ConvertLocalizedStringToJson<List<Guid>>(),
                         ConvertJsonToLocalizedString<List<Guid>>());
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

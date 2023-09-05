﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTableName("RosasClients");
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.UniqueName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Title).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

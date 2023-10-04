using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SpecificationValueConfiguration : BaseEntityConfiguration<SpecificationValue>, IEntityTypeConfiguration<SpecificationValue>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SpecificationValue> builder)
        {
            builder.ToTableName("RosasSpecificationValues");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Data).IsRequired().HasMaxLength(500).IsUnicode();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.HasOne(b => b.Field).WithMany(p => p.Values).HasForeignKey(f => f.FieldId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Tenant).WithMany(p => p.SpecificationsValues).HasForeignKey(f => f.FieldId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTableName("RosasFeatures");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(250).IsUnicode();
            builder.Property(r => r.DisplayName).IsRequired(false).HasMaxLength(250).IsUnicode();
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Product).WithMany(p => p.Features).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

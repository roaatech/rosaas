using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

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
            builder.Property(r => r.UniqueName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Title).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.Url).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreationEndpoint).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.ActivationEndpoint).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.DeactivationEndpoint).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.DeletionEndpoint).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.EditedByUserId).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.Edited).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Client).WithMany(p => p.Products).HasForeignKey(f => f.ClientId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

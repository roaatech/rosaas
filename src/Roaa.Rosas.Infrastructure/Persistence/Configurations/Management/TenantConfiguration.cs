using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("mng_Tenants");
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.UniqueName).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Title).IsRequired(false).HasMaxLength(250);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.EditedByUserId).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.Edited).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Product).WithMany(p => p.Tenants).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

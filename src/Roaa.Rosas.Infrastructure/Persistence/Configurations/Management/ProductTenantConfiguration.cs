using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class ProductTenantConfiguration : IEntityTypeConfiguration<ProductTenant>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<ProductTenant> builder)
        {
            builder.ToTable("mng_ProductTenants");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.Tenant).WithMany(p => p.Products).HasForeignKey(f => f.TenantId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Product).WithMany(p => p.Tenants).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

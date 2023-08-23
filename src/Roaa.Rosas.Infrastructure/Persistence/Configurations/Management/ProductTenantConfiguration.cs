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
            builder.ToTableName("RosasProductTenants");
            builder.HasKey(x => x.Id);
            builder.HasOne(b => b.Tenant).WithMany(p => p.Products).HasForeignKey(f => f.TenantId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Product).WithMany(p => p.Tenants).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.HealthCheckStatus).WithOne(p => p.ProductTenant).HasForeignKey<TenantHealthStatus>(e => e.Id).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.HealthCheckUrl).IsRequired(true).HasMaxLength(250);
            builder.Property(r => r.HealthCheckUrlIsOverridden).IsRequired(true);
            builder.Property(r => r.EditedByUserId).IsRequired();
            builder.Property(r => r.Edited).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

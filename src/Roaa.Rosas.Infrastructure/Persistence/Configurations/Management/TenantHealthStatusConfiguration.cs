using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantHealthStatusConfiguration : IEntityTypeConfiguration<TenantHealthStatus>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantHealthStatus> builder)
        {
            builder.ToTableName("RosasTenantHealthStatuses");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.IsHealthy).IsRequired();
            builder.Property(r => r.HealthCheckUrl).IsRequired().HasMaxLength(250);
            builder.Property(r => r.LastCheckDate).IsRequired();
            builder.Property(r => r.CheckDate).IsRequired();
            builder.Property(r => r.Duration).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }

}

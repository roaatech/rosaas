using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantHealthCheckConfiguration : IEntityTypeConfiguration<TenantHealthCheckHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantHealthCheckHistory> builder)
        {
            builder.ToTableName("RosasTenantHealthCheckHistory");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.Duration).IsRequired();
            builder.Property(r => r.IsHealthy).IsRequired();
            builder.Property(r => r.TimeStamp).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.HealthCheckUrl).IsRequired().HasMaxLength(250);
            builder.Property(r => r.TimeStamp).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }

}

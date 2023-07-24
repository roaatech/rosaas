using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantProcessConfiguration : IEntityTypeConfiguration<TenantProcess>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantProcess> builder)
        {
            builder.ToTableName("RosasTenantProcesses");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.PreviousStatus).IsRequired();
            builder.Property(r => r.OwnerId).IsRequired();
            builder.Property(r => r.OwnerType).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.Created).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Property(r => r.Message).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
    public class TenantAvailabilityConfiguration : IEntityTypeConfiguration<TenantHealthCheck>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantHealthCheck> builder)
        {
            builder.ToTableName("RosasTenantHealthChecks");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.Duration).IsRequired();
            builder.Property(r => r.IsHealthy).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.HealthCheckUrl).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Created).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
    public class JobTaskConfiguration : IEntityTypeConfiguration<JobTask>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<JobTask> builder)
        {
            builder.ToTableName("RosasJobTasks");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.Type).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.Created).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

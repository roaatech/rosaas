using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
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

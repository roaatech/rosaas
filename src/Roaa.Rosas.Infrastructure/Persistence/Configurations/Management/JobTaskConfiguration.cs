using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

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
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.CreationDate).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTableName("RosasSettings");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Key).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Value).IsRequired().HasMaxLength(250);

            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

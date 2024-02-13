using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
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

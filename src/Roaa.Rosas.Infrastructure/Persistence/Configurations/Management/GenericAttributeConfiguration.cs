using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class GenericAttributeConfiguration : IEntityTypeConfiguration<GenericAttribute>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<GenericAttribute> builder)
        {
            builder.ToTableName("RosasGenericAttributes");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Key).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Value).IsRequired().HasMaxLength(1000);
            builder.Property(r => r.KeyGroup).IsRequired().HasMaxLength(250);

            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

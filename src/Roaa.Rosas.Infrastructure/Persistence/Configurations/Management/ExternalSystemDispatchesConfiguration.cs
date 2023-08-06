using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class ExternalSystemDispatchesConfiguration : IEntityTypeConfiguration<ExternalSystemDispatch>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<ExternalSystemDispatch> builder)
        {
            builder.ToTableName("RosasExternalSystemDispatches");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.IsSuccessful).IsRequired();
            builder.Property(r => r.Url).IsRequired().HasMaxLength(250);
            builder.Property(r => r.Duration).IsRequired();
            builder.Property(r => r.DispatchDate).IsRequired();
            builder.Property(r => r.TimeStamp).HasConversion(
                v => v.Ticks,
                v => new DateTime(v)
            );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }

}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantProcessHistoryConfiguration : BaseEntityConfiguration<TenantProcessHistory>, IEntityTypeConfiguration<TenantProcessHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantProcessHistory> builder)
        {
            builder.ToTableName("RosasTenantProcessHistory");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TenantId).IsRequired();
            builder.Property(r => r.ProductId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.Step).IsRequired();
            builder.Property(r => r.ExpectedResourceStatus).IsRequired();
            builder.Property(r => r.ProcessType).IsRequired();
            builder.Property(r => r.OwnerType).IsRequired();
            builder.Property(r => r.Data).IsRequired(false);
            builder.Property(r => r.Enabled).IsRequired();
            builder.Property(r => r.ProcessDate).IsRequired();
            builder.Property(r => r.TimeStamp).IsRequired().HasConversion(
               v => v.Ticks,
               v => new DateTime(v)
           );
            builder.Property(r => r.Notes)
                  .HasMaxLength(1000)
                  .IsRequired(false)
                  .IsUnicode()
                  .HasConversion(
                          ConvertLocalizedStringToJson<ICollection<ProcessNote>>(),
                          ConvertJsonToLocalizedString<ICollection<ProcessNote>>()
                   );
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

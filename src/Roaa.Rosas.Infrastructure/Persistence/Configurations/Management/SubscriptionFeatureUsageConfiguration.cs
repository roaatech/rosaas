using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionFeatureUsageConfiguration : IEntityTypeConfiguration<SubscriptionFeatureUsage>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionFeatureUsage> builder)
        {
            builder.ToTableName("RosasSubscriptionFeatureUsages");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

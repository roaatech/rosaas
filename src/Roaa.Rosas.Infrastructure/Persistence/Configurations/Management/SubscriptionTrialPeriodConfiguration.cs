using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionTrialPeriodConfiguration : IEntityTypeConfiguration<TrialSubscription>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TrialSubscription> builder)
        {
            builder.ToTableName("RosasTrialSubscriptions");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.TrialPlanId).IsRequired();
            builder.Property(r => r.TrialPlanPriceId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.StartDate).IsRequired(true);
            builder.Property(r => r.EndDate).IsRequired(true);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

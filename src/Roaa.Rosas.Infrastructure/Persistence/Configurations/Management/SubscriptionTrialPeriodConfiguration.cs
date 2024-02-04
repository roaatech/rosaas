using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionTrialPeriodConfiguration : IEntityTypeConfiguration<SubscriptionTrialPeriod>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionTrialPeriod> builder)
        {
            builder.ToTableName("RosasSubscriptionTrialPeriods");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.PlanId).IsRequired();
            builder.Property(r => r.PlanPriceId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.StartDate).IsRequired(true);
            builder.Property(r => r.EndDate).IsRequired(false);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

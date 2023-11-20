using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionAutoRenewalHistoryConfiguration : IEntityTypeConfiguration<SubscriptionAutoRenewalHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionAutoRenewalHistory> builder)
        {
            builder.ToTableName("RosasSubscriptionAutoRenewalHistories");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.PlanId).IsRequired();
            builder.Property(r => r.PlanPriceId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.PlanCycle).IsRequired();
            builder.Property(r => r.Price).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.RenewalDate).IsRequired(true);
            builder.Property(r => r.AutoRenewalEnabledDate).IsRequired(true);
            builder.Property(r => r.AutoRenewalEnabledByUserId).IsRequired(true);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

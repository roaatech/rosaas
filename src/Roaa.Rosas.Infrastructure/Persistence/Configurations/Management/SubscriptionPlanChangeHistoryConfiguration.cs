using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SubscriptionPlanChangeHistoryConfiguration : IEntityTypeConfiguration<SubscriptionPlanChangeHistory>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<SubscriptionPlanChangeHistory> builder)
        {
            builder.ToTableName("RosasSubscriptionPlanChangeHistories");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.PlanId).IsRequired();
            builder.Property(r => r.PlanPriceId).IsRequired();
            builder.Property(r => r.SubscriptionId).IsRequired();
            builder.Property(r => r.PlanCycle).IsRequired();
            builder.Property(r => r.Type).IsRequired();
            builder.Property(r => r.Price).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.ChangeDate).IsRequired(true);
            builder.Property(r => r.PlanChangeEnabledDate).IsRequired(true);
            builder.Property(r => r.PlanChangeEnabledByUserId).IsRequired(true);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

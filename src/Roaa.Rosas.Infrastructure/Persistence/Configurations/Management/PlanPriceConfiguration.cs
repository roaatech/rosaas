using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class PlanPriceConfiguration : IEntityTypeConfiguration<PlanPrice>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<PlanPrice> builder)
        {
            builder.ToTableName("RosasPlanPrices");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.PlanCycle).IsRequired();
            builder.Property(r => r.Price).HasPrecision(8, 2).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Plan).WithMany(p => p.Prices).HasForeignKey(f => f.PlanId).OnDelete(DeleteBehavior.Restrict);
        }
        #endregion
    }
}

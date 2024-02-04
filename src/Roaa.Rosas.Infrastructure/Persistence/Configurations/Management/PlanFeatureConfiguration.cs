using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class PlanFeatureConfiguration : BaseEntityConfiguration<Specification>, IEntityTypeConfiguration<PlanFeature>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<PlanFeature> builder)
        {
            builder.ToTableName("RosasPlanFeatures");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
            builder.HasOne(b => b.Plan).WithMany(p => p.Features).HasForeignKey(f => f.PlanId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Feature).WithMany(p => p.Plans).HasForeignKey(f => f.FeatureId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(r => r.UnitDisplayName)
                      .HasMaxLength(150)
                      .IsRequired(false)
                      .IsUnicode()
                      .HasConversion(
                              ConvertLocalizedStringToJson<LocalizedString>(),
                              ConvertJsonToLocalizedString<LocalizedString>()
                       );
        }
        #endregion
    }
}

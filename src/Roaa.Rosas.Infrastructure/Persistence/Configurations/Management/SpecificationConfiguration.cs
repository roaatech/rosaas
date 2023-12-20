using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class SpecificationConfiguration : BaseEntityConfiguration<Specification>, IEntityTypeConfiguration<Specification>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Specification> builder)
        {
            builder.ToTableName("RosasSpecifications");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.SystemName).IsRequired().HasMaxLength(50).IsUnicode();
            builder.Property(r => r.NormalizedSystemName).IsRequired().HasMaxLength(50).IsUnicode();
            builder.Property(r => r.DisplayName)
                   .HasMaxLength(150)
                   .IsRequired()
                   .IsUnicode()
                   .HasConversion(
                           ConvertLocalizedStringToJson<LocalizedString>(),
                           ConvertJsonToLocalizedString<LocalizedString>()
                    );
            builder.Property(r => r.Description)
                  .HasMaxLength(1000)
                  .IsRequired(false)
                  .IsUnicode()
                  .HasConversion(
                          ConvertLocalizedStringToJson<LocalizedString>(),
                          ConvertJsonToLocalizedString<LocalizedString>()
                   );
            builder.Property(r => r.InputType).IsRequired();
            builder.Property(r => r.DataType).IsRequired();
            builder.Property(r => r.RegularExpression).IsRequired(false).HasMaxLength(250).IsUnicode();
            builder.Property(r => r.ValidationFailureDescription)
                 .HasMaxLength(250)
                 .IsRequired(false)
                 .IsUnicode()
                 .HasConversion(
                         ConvertLocalizedStringToJson<LocalizedString>(),
                         ConvertJsonToLocalizedString<LocalizedString>()
                  );
            builder.Property(r => r.IsRequired).IsRequired();
            builder.Property(r => r.IsUserEditable).IsRequired();
            builder.Property(r => r.IsPublished).IsRequired();
            builder.Property(r => r.IsSubscribed).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.HasOne(b => b.Product).WithMany(p => p.Specifications).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

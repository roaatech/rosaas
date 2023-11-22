using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<User>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTableName("IdentityUsers");
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Property(r => r.IsDeleted).HasDefaultValue(false).IsRequired();
            builder.Property(r => r.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(r => r.Locale).HasMaxLength(2);
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class EntityAdminPrivilegeConfiguration : IEntityTypeConfiguration<EntityAdminPrivilege>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<EntityAdminPrivilege> builder)
        {
            builder.ToTableName("RosasEntityAdminPrivileges");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.IsMajor).IsRequired();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.ModifiedByUserId).IsRequired();
            builder.Property(r => r.CreationDate).IsRequired();
            builder.Property(r => r.ModificationDate).IsRequired();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

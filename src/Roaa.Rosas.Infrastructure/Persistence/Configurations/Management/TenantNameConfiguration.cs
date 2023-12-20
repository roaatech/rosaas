using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantNameConfiguration : IEntityTypeConfiguration<TenantName>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantName> builder)
        {
            builder.ToTableName("RosasTenantNames");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(250);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

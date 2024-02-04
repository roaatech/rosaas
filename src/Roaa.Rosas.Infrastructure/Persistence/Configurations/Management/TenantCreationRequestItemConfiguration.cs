using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;
using Roaa.Rosas.Infrastructure.Persistence.Configurations.Shared;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class TenantCreationRequestSpecificationConfiguration : BaseEntityConfiguration<TenantCreationRequestSpecification>, IEntityTypeConfiguration<TenantCreationRequestSpecification>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<TenantCreationRequestSpecification> builder)
        {
            builder.ToTableName("RosasTenantCreationRequestSpecifications");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Value).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.HasOne(b => b.TenantCreationRequest).WithMany(p => p.Specifications).HasForeignKey(f => f.TenantCreationRequestId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

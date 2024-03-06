using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class LinkedCardConfiguration : IEntityTypeConfiguration<LinkedCard>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<LinkedCard> builder)
        {
            builder.ToTableName("RosasLinkedCards");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.ReferenceId).IsRequired(true).HasMaxLength(100).IsUnicode();
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

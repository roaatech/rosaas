using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        #region Configure 
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTableName("RosasPlans");
            builder.HasKey(x => x.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(250).IsUnicode();
            builder.Property(r => r.Description).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.Property(r => r.CreatedByUserId).IsRequired();
            builder.Property(r => r.EditedByUserId).IsRequired();
            builder.Property(r => r.Created).IsRequired();
            builder.Property(r => r.Edited).IsRequired();
            builder.HasOne(b => b.Product).WithMany(p => p.Plans).HasForeignKey(f => f.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(r => r.DomainEvents);
        }
        #endregion
    }
}

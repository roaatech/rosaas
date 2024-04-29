using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Infrastructure.Common;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Identity
{
    public class WebhookEndpointConfiguration : IEntityTypeConfiguration<WebhookEndpoint>
    {
        public void Configure(EntityTypeBuilder<WebhookEndpoint> builder)
        {
            #region Configure 


            builder.ToTableName("RosasWebhookEndpoints");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Url).IsRequired(true);
            builder.Property(x => x.SigningSecret).IsRequired(true);
            builder.Property(x => x.Description).IsRequired();
            builder.HasMany(e => e.EventsToListen)
                              .WithOne(e => e.WebhookEndpoint)
                              .HasForeignKey(e => e.WebhookEndpointId)
                              .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.EntityId).IsRequired(true);
            builder.Property(x => x.EntityType).IsRequired(true);
            builder.Property(r => r.CreatedByUserId).IsRequired(true);
            builder.Property(r => r.ModifiedByUserId).IsRequired(true);
            builder.Property(r => r.CreationDate).IsRequired(true);
            builder.Property(r => r.ModificationDate).IsRequired(true);
            builder.Ignore(r => r.DomainEvents);
            #endregion
        }


    }

}


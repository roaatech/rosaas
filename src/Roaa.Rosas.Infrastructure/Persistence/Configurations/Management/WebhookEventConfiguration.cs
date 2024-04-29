using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Infrastructure.Persistence.Configurations.Management
{
    public class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEndpointEvent>
    {
        public void Configure(EntityTypeBuilder<WebhookEndpointEvent> builder)
        {
            #region Configure 

            builder.ToTable("RosasWebhookEvents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Event).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.HasOne(x => x.WebhookEndpoint)
                    .WithMany(e => e.EventsToListen)
                    .HasForeignKey(x => x.WebhookEndpointId)
                    .OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(r => r.DomainEvents);
            #endregion

        }
    }
}

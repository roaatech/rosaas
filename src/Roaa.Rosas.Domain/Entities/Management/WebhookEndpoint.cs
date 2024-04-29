using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class WebhookEndpoint : BaseAuditableEntity
    {
        public string Url { get; set; } = string.Empty;
        public string SigningSecret { get; set; } = string.Empty;
        public virtual ICollection<WebhookEndpointEvent> EventsToListen { get; set; } = new List<WebhookEndpointEvent>();
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
    }

}
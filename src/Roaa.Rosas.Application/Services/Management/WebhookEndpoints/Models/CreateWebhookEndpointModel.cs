using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models
{
    public class CreateWebhookEndpointModel
    {
        public string Url { get; set; } = string.Empty;
        public string SigningSecret { get; set; } = string.Empty;
        public List<WebhookEvents> EventsToListen { get; set; } = new List<WebhookEvents>();
        public string Description { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public bool IsActive { get; set; }
    }
}
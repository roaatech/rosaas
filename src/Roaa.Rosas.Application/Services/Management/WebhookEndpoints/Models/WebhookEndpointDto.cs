using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models
{
    public class WebhookEndpointDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string SigningSecret { get; set; } = string.Empty;
        public List<WebhookEventDto>? EventsToListen { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }

    }

    public class WebhookEventDto
    {
        public Guid Id { get; set; }
        public WebhookEvents Event { get; set; }
    }

}

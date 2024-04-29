using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models
{
    public record WebhookEndpointsListItem
    {
        public Guid Id { get; init; }
        public string Url { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Guid EntityId { get; init; }
        public bool IsActive { get; init; }
        public List<WebhookEvents> EventsToListen { get; init; } = new List<WebhookEvents>();
        public string SigningSecret { get; set; } = string.Empty;
        public EntityType EntityType { get; set; }

    }


}

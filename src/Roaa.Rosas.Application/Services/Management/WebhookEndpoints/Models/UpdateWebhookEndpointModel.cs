using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models
{
    public class UpdateWebhookEndpointModel
    {
        public string Url { get; set; } = string.Empty;
        public string SigningSecret { get; set; } = string.Empty;
        public List<WebhookEvents> EventsToListen { get; set; } = new List<WebhookEvents>();
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

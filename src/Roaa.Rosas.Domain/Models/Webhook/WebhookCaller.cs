using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Models.Webhook
{
    public record WebhookCaller<T>
    {
        public T Payload { get; set; }
        public List<WebhookEndpoint> WebhookEndpoints { get; set; }

    }

    public record GlobalPayload<TPayload>
    {
        public string TenantSystemName { get; set; } = string.Empty;
        public WebhookEvents EventType { get; set; }
        public TPayload? MetaData { get; set; }
    }
    public record WebhookEndpoint
    {
        public string Url { get; set; } = string.Empty;
        public string? Secret { get; set; } = string.Empty;
    }

}

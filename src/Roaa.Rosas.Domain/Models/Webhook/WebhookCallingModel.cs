using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Models.Webhook
{
    public record WebhookCallingModel<T> where T : class, new()
    {
        public T Payload { get; set; } = new T();
        public List<WebhookEndpointModel> WebhookEndpoints { get; set; } = new List<WebhookEndpointModel>();

    }

    public record GlobalPayload<TPayload>
    {
        public string TenantSystemName { get; set; } = string.Empty;
        public WebhookEvents EventType { get; set; }
        public TPayload? MetaData { get; set; }
    }
    public record WebhookEndpointModel
    {
        public string Url { get; set; } = string.Empty;
        public string? Secret { get; set; } = string.Empty;
    }

}

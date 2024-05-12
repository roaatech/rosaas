using Roaa.Rosas.Domain.Models.Webhook;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IWebhookAPI
    {
        Task CallWebhookEndpointsAsync<TPayload>(WebhookCaller<GlobalPayload<TPayload>> model, CancellationToken cancellationToken = default);
    }
}

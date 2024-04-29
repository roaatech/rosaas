using Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models;
using Roaa.Rosas.Common.Models.Results;




namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints
{
    public interface IWebhookEndpointService
    {
        Task<Result<List<WebhookEndpointsListItem>>> GetWebhookEndpointsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<Result<WebhookEndpointDto>> GetWebhookEndpointByIdAsync(Guid webhookEndpointId, CancellationToken cancellationToken = default);
        Task<Result<CreatedResult<Guid>>> CreateWebhookEndpointAsync(CreateWebhookEndpointModel model, CancellationToken cancellationToken = default);
        Task<Result> ChangeWebhookEndpointStatusAsync(Guid webhookEndpointId, bool isActive, CancellationToken cancellationToken = default);
        Task<Result> UpdateWebhookEndpointAsync(Guid webhookEndpointId, UpdateWebhookEndpointModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteWebhookEndpointAsync(Guid webhookEndpointId, CancellationToken cancellationToken = default);
    }
}
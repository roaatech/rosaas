using Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;




namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints
{
    public interface IWebhookEndpointService
    {
        Task<Result<List<WebhookEndpointsListItem>>> GetWebhookEndpointsByEntityIdAsync(Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
        Task<Result<WebhookEndpointDto>> GetWebhookEndpointByIdAsync(Guid webhookEndpointId, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
        Task<Result<CreatedResult<Guid>>> CreateWebhookEndpointAsync(CreateWebhookEndpointModel model, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
        Task<Result> ChangeWebhookEndpointStatusAsync(Guid webhookEndpointId, bool isActive, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
        Task<Result> UpdateWebhookEndpointAsync(Guid webhookEndpointId, UpdateWebhookEndpointModel model, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
        Task<Result> DeleteWebhookEndpointAsync(Guid webhookEndpointId, Guid entityId, EntityType entityType, CancellationToken cancellationToken = default);
    }
}
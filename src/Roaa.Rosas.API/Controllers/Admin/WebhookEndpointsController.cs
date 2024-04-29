using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.WebhookEndpoints;
using Roaa.Rosas.Application.Services.Management.WebhookEndpoints.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;


namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    [Authorize(Policy = AuthPolicy.Management.WebhookEndpoints)]
    public class WebhookEndpointsController : BaseManagementApiController
    {
        private readonly IWebhookEndpointService _webhookEndpointService;
        private readonly IIdentityContextService _identityContextService;

        public WebhookEndpointsController(IWebhookEndpointService webhookEndpointService, IIdentityContextService identityContextService)
        {
            _webhookEndpointService = webhookEndpointService;
            _identityContextService = identityContextService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetWebhookEndpointsByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _webhookEndpointService.GetWebhookEndpointsByProductIdAsync(productId, cancellationToken));
        }
        [HttpGet("{webhookEndpointId}")]
        public async Task<IActionResult> GetWebhookEndpointByIdAsync([FromRoute] Guid webhookEndpointId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _webhookEndpointService.GetWebhookEndpointByIdAsync(webhookEndpointId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateWebhookEndpointAsync([FromBody] CreateWebhookEndpointModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _webhookEndpointService.CreateWebhookEndpointAsync(model, cancellationToken));
        }

        [HttpPut("{webhookEndpointId}")]
        public async Task<IActionResult> UpdateWebhookEndpointAsync([FromBody] UpdateWebhookEndpointModel model, [FromRoute] Guid productId, [FromRoute] Guid webhookEndpointId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _webhookEndpointService.UpdateWebhookEndpointAsync(webhookEndpointId, model, cancellationToken));
        }

        [HttpPut("{webhookEndpointId}/change-status")]
        public async Task<IActionResult> ChangeWebhookEndpointStatusAsync([FromRoute] Guid productId, [FromRoute] Guid webhookEndpointId, [FromBody] WebhookEndpointStatusModel statusModel, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _webhookEndpointService.ChangeWebhookEndpointStatusAsync(webhookEndpointId, statusModel.IsActive, cancellationToken));
        }



        [HttpDelete("{webhookEndpointId}")]
        public async Task<IActionResult> DeleteWebhookEndpointAsync([FromRoute] Guid productId, [FromRoute] Guid webhookEndpointId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _webhookEndpointService.DeleteWebhookEndpointAsync(webhookEndpointId, cancellationToken));
        }
    }
}

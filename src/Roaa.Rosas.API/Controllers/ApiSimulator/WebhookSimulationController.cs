using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Controllers;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Framework.Controllers.ApiSimulator
{
    [Route("webhook-simulator")]
    public class WebhookSimulationController : BaseApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public WebhookSimulationController(ISender mediator,
                                           IIdentityContextService identityContextService)
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
        }
        #endregion


        #region Actions    
        [HttpPost("notifyme")]
        public async Task<IActionResult> NotifymeAsync(GlobalPayloadRequest model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }

        #endregion



        public record GlobalPayloadRequest
        {
            public string TenantSystemName { get; set; } = string.Empty;
            public WebhookEvents EventType { get; set; }
            public dynamic? MetaData { get; set; }
        }
        public record WebhookEndpointRequestModel
        {
            public string Url { get; set; } = string.Empty;
            public string? Secret { get; set; } = string.Empty;
        }
    }
}
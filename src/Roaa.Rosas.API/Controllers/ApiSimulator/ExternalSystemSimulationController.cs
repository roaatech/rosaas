using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Controllers;

namespace Roaa.Rosas.Framework.Controllers.ApiSimulator
{
    [Route("external-system-simulator/tenants")]
    public class ExternalSystemSimulationController : BaseApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public ExternalSystemSimulationController(ISender mediator,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
        }
        #endregion


        #region Actions    
        [HttpPost("cerated")]
        public async Task<IActionResult> CreateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }


        [HttpPost("active")]
        public async Task<IActionResult> ActivateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }


        [HttpPost("inactive")]
        public async Task<IActionResult> DeactivateTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }


        [HttpPost("deleted")]
        public async Task<IActionResult> DeleteTenantAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }


        [HttpGet("{name}/health-check")]
        public async Task<IActionResult> HealthCheckAsync(string name, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }

        [HttpPost("health-status-unhealthy")]
        public async Task<IActionResult> InformHealthStatusUnhealthyAsync(TenantModel model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok();
        }
        #endregion



        public record TenantModel
        {
            public string? TenantName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }
    }
}
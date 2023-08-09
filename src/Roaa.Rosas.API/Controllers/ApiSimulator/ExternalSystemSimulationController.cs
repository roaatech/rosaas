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
        [HttpPost()]
        public async Task<IActionResult> CreateTenantAsync(CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpPut("{tenantId}/status/active")]
        public async Task<IActionResult> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpPut("{tenantId}/status/inactive")]
        public async Task<IActionResult> DeactivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpDelete("{tenantId}")]
        public async Task<IActionResult> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpGet("{tenantId}/health-check")]
        public async Task<IActionResult> HealthCheckAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpPost("{tenantId}/health-status-change")]
        public async Task<IActionResult> HealthStatusChangeAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return Ok();
        }
        #endregion
    }
}
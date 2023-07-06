using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Controllers;

namespace Roaa.Rosas.Framework.Controllers.ApiSimulator
{
    [Route("Simulator/Rosas/v1/tenants")]
    public class SimulationTenantsController : BaseApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public SimulationTenantsController(ISender mediator,
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


        [HttpPut("{id}/status/active")]
        public async Task<IActionResult> ActivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpPut("{id}/status/deactive")]
        public async Task<IActionResult> DeactivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        #endregion
    }
}
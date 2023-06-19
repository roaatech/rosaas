using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusById;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{
    [Route("api/management/apps/v1/[controller]")]
    public class TenantsController : BaseExternalSystemApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public TenantsController(ISender mediator,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
        }
        #endregion


        #region Actions   


        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetTenantStatusByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetTenantStatusByIdQuery(id, _identityContextService.GetProductId()), cancellationToken));
        }


        //[HttpPost()]
        //public async Task<IActionResult> CreateTenantAsync(Guid productId, [FromBody] ActivateTenantCommand command, CancellationToken cancellationToken = default)
        //{
        //    return EmptyResult(await _mediator.Send(command, cancellationToken));
        //}

        [HttpPut("{id}/status/created")]
        public async Task<IActionResult> SetTenantAsCreatedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.CreatedAsActive), cancellationToken));
        }

        [HttpPut("{id}/status/active")]
        public async Task<IActionResult> ActivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Active), cancellationToken));
        }


        [HttpPut("{id}/status/deactive")]
        public async Task<IActionResult> DeactivateTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Deactive), cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Deleted), cancellationToken));
        }
        #endregion
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Tenants.Commands.UpdateTenantMetadata;
using Roaa.Rosas.Application.Tenants.Queries.GetTenantMetadataById;
using Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusById;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.ResponseMessages;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Education.API.Models.Common.Responses;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{
    [Route("api/management/apps/v1/tenants")]
    public class ExternalSystemTenantsController : BaseExternalSystemApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;

        #endregion

        #region Corts
        public ExternalSystemTenantsController(ISender mediator,
                                 IIdentityContextService identityContextService
                                 )
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
        }
        #endregion


        #region Actions   


        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetTenantStatusByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetTenantStatusByIdQuery(id, _identityContextService.GetProductId()), cancellationToken));
        }


        //[HttpPost()]
        //public async Task<IActionResult> CreateTenantAsync(Guid productId, [FromBody] ActivateTenantCommand command, CancellationToken cancellationToken = default)
        //{
        //    return EmptyResult(await _mediator.Send(command, cancellationToken));
        //}

        [HttpPut("{id}/status/created")]
        public async Task<IActionResult> SetTenantAsCreatedAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.CreatedAsActive, _identityContextService.GetProductId()), cancellationToken));
        }

        [HttpPut("{id}/status/active")]
        public async Task<IActionResult> ActivateTenantAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Active, _identityContextService.GetProductId()), cancellationToken));
        }


        [HttpPut("{id}/status/deactive")]
        public async Task<IActionResult> DeactivateTenantAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Deactive, _identityContextService.GetProductId()), cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenantAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(id, TenantStatus.Deleted, _identityContextService.GetProductId()), cancellationToken));
        }


        [HttpPut("{id}/metadata")]
        public async Task<IActionResult> UpdateTenantMetadataAsync([FromRoute] Guid id, dynamic metadata, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new UpdateTenantMetadataCommand(id, _identityContextService.GetProductId(), metadata), cancellationToken));
        }


        [HttpGet("{id}/metadata")]
        public async Task<IActionResult> GetTenantMetadataAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTenantMetadataByIdQuery(id, _identityContextService.GetProductId()), cancellationToken);

            var response = new ResponseItemResult<dynamic>
            {
                Metadata = new ResponseMetadata(),
                Data = JsonConvert.DeserializeObject<dynamic>(result.Data.Metadata),
            };

            return Content(JsonConvert.SerializeObject(response), "application/json");
        }
        #endregion
    }
}

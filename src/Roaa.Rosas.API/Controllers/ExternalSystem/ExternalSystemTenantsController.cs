using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenantMetadata;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantMetadataByName;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantStatusByName;
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


        [HttpGet("{name}/status")]
        public async Task<IActionResult> GetTenantStatusByIdAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetTenantStatusByNameQuery(name, _identityContextService.GetProductId()), cancellationToken));
        }

        [HttpPost("{name}/created")]
        public async Task<IActionResult> SetTenantAsCreatedAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(name, TenantStatus.CreatedAsActive, _identityContextService.GetProductId(), string.Empty), cancellationToken));
        }

        [HttpPost("{name}/active")]
        public async Task<IActionResult> ActivateTenantAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(name, TenantStatus.Active, _identityContextService.GetProductId(), string.Empty), cancellationToken));
        }


        [HttpPost("{name}/inactive")]
        public async Task<IActionResult> DeactivateTenantAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(name, TenantStatus.Deactive, _identityContextService.GetProductId(), string.Empty), cancellationToken));
        }


        [HttpPost("{name}/deleted")]
        public async Task<IActionResult> DeleteTenantAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new ChangeTenantStatusCommand(name, TenantStatus.Deleted, _identityContextService.GetProductId(), string.Empty), cancellationToken));
        }


        [HttpPost("{name}/metadata")]
        public async Task<IActionResult> UpdateTenantMetadataAsync([FromRoute] string name, dynamic metadata, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new UpdateTenantMetadataCommand(name, _identityContextService.GetProductId(), metadata), cancellationToken));
        }


        [HttpGet("{name}/metadata")]
        public async Task<IActionResult> GetTenantMetadataAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetTenantMetadataByNameQuery(name, _identityContextService.GetProductId()), cancellationToken);

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

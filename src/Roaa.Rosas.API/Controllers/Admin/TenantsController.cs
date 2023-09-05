using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.DeleteTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenant;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantById;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantProcessesByTenantId;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsLookupList;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsPaginatedList;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class TenantsController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<TenantsController> _logger;
        private readonly ITenantService _tenantService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public TenantsController(ILogger<TenantsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                                ITenantService tenantService,
                                ISender mediator)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _mediator = mediator;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetTenantsPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _mediator.Send(new GetTenantsPaginatedListQuery(pagination, filters, sort), cancellationToken));
        }


        [HttpGet("Lookup")]
        public async Task<IActionResult> GetProductsLookupListAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetTenantsLookupListQuery(), cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetTenantByIdQuery(id), cancellationToken));
        }


        [HttpGet("{id}/Products/{productId}/processes")]
        public async Task<IActionResult> GetTenantProcessesListByTenantIdAsync([FromQuery] PaginationMetaData pagination, [FromRoute] Guid productId, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _mediator.Send(new GetTenantProcessesByTenantIdQuery(id, productId, pagination), cancellationToken));
        }



        [HttpGet($"/{PrefixSuperAdminMainApiRoute}/products/{{productId}}/Tenants")]
        public async Task<IActionResult> GetProductTenantsListAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetSubscriptionsListQuery(productId), cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantCommand command, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }

        [HttpPut("Status")]
        public async Task<IActionResult> UpdateTenantStatusAsync([FromBody] ChangeTenantStatusByIdCommand command, CancellationToken cancellationToken = default)
        {
            var ddd = await _mediator.Send(command, cancellationToken);
            return ItemResult(ddd);
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteTenantAsync([FromBody] DeleteResourceModel<Guid> model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(new DeleteTenantCommand(model.Id), cancellationToken));
        }
        #endregion 
    }
}

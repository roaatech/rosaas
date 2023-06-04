using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants;
using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class TenantController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<TenantController> _logger;
        private readonly ITenantService _tenantService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public TenantController(ILogger<TenantController> logger,
                                IWebHostEnvironment environment,
                                ITenantService tenantService)
        {
            _logger = logger;
            _environment = environment;
            _tenantService = tenantService;
        }
        #endregion


        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetTenantsPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _tenantService.GetTenantsPaginatedListAsync(pagination, filters, sort, cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _tenantService.GetTenantByIdAsync(id, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantModel model, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _tenantService.CreateTenantAsync(model, cancellationToken));
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantModel model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _tenantService.UpdateTenantAsync(model, cancellationToken));
        }

        [HttpPut("Status")]
        public async Task<IActionResult> UpdateTenantStatusAsync([FromBody] UpdateTenantStatusModel model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _tenantService.UpdateTenantStatusAsync(model, cancellationToken));
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteTenantAsync([FromBody] DeleteResourceModel<Guid> model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _tenantService.DeleteTenantAsync(model, cancellationToken));
        }
        #endregion


    }
}

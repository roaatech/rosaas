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
        public async Task<IActionResult> GetTenantsPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken)
        {
            return PaginatedResult(await _tenantService.GetTenantsPaginatedListAsync(pagination, filters, sort));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantByIdAsync([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return ItemResult(await _tenantService.GetTenantByIdAsync(id));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantModel model, CancellationToken cancellationToken)
        {
            return ItemResult(await _tenantService.CreateTenantAsync(model));
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateTenantAsync([FromBody] UpdateTenantModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _tenantService.UpdateTenantAsync(model));
        }
        #endregion


    }
}

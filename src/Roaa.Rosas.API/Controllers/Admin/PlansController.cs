using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Plans;
using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    public class PlansController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<PlansController> _logger;
        private readonly IPlanService _planService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public PlansController(ILogger<PlansController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IPlanService planService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _planService = planService;
        }
        #endregion

        #region Actions   

        [HttpGet($"/{PrefixSuperAdminMainApiRoute}/[controller]")]
        public async Task<IActionResult> GetPlansListByProductIdAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _planService.GetPlansPaginatedListAsync(pagination, filters, sort, cancellationToken));
        }


        [HttpGet("Lookup")]
        public async Task<IActionResult> GetPlansLookupListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planService.GetPlansLookupListByProductIdAsync(productId, cancellationToken));
        }


        [HttpGet()]
        public async Task<IActionResult> GetPlansListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planService.GetPlansListByProductIdAsync(productId, cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanByIdAsync([FromRoute] Guid productId, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planService.GetPlanByIdAsync(id, productId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreatePlanAsync([FromBody] CreatePlanModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planService.CreatePlanAsync(model, productId, cancellationToken));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanAsync([FromBody] UpdatePlanModel model, [FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planService.UpdatePlanAsync(id, model, productId, cancellationToken));
        }



        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishPlanAsync([FromBody] PublishPlanModel model, [FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planService.PublishPlanAsync(id, model, productId, cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanAsync([FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planService.DeletePlanAsync(id, productId, cancellationToken));
        }

        #endregion

    }
}

using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.GeneralPlans;
using Roaa.Rosas.Application.Services.Management.GeneralPlans.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [NonController]
    [Authorize(Policy = AuthPolicy.Management.GeneralPlans, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class GeneralPlansController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<GeneralPlansController> _logger;
        private readonly IPlanService _planService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public GeneralPlansController(ILogger<GeneralPlansController> logger,
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

        [HttpGet()]
        public async Task<IActionResult> GetPlansPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _planService.GetPlansPaginatedListAsync(pagination, filters, sort, cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planService.GetPlanByIdAsync(id, cancellationToken));
        }



        [HttpPost()]
        public async Task<IActionResult> CreatePlanAsync([FromBody] CreatePlanModel model, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planService.CreatePlanAsync(model, cancellationToken));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanAsync([FromBody] UpdatePlanModel model, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planService.UpdatePlanAsync(id, model, cancellationToken));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planService.DeletePlanAsync(id, cancellationToken));
        }
        #endregion


    }
}

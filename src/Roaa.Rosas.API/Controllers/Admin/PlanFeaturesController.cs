using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.PlanFeatures;
using Roaa.Rosas.Application.Services.Management.PlanFeatures.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/plans")]
    public class PlanFeaturesController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<PlanFeaturesController> _logger;
        private readonly IPlanFeatureService _planFeatureService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public PlanFeaturesController(ILogger<PlanFeaturesController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IPlanFeatureService planFeatureService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _planFeatureService = planFeatureService;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetPlanFeaturesListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planFeatureService.GetPlanFeaturesListByProductIdAsync(productId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreatePlanFeatureAsync([FromBody] CreatePlanFeatureModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planFeatureService.CreatePlanFeatureAsync(model, productId, cancellationToken));
        }


        [HttpPut("{planFeatureId}")]
        public async Task<IActionResult> UpdatePlanFeatureAsync([FromBody] UpdatePlanFeatureModel model, [FromRoute] Guid planFeatureId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planFeatureService.UpdatePlanFeatureAsync(planFeatureId, model, productId, cancellationToken));
        }


        [HttpDelete("{planFeatureId}")]
        public async Task<IActionResult> DeletePlanFeatureAsync([FromRoute] Guid planFeatureId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planFeatureService.DeletePlanFeatureAsync(planFeatureId, productId, cancellationToken));
        }

        #endregion


    }
}

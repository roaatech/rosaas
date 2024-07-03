using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.PlanFeatures;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class PlanFeaturesController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<PlanFeaturesController> _logger;
        private readonly IPlanFeatureService _planFeatureService;
        #endregion


        #region Corts
        public PlanFeaturesController(ILogger<PlanFeaturesController> logger,
                                      IPlanFeatureService planFeatureService)
        {
            _logger = logger;
            _planFeatureService = planFeatureService;
        }
        #endregion


        #region Actions    

        [HttpGet("productOwner/{productOwnerName}/Product/{productName}/[controller]")]
        public async Task<IActionResult> GetPlanFeaturesListByProductNameAsync([FromRoute] string productOwnerName, string productName, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planFeatureService.GetPublishedPlanFeaturesListByProductNameAsync(productOwnerName, productName, cancellationToken));
        }


        [HttpGet("productOwner/{productOwnerName}/Product/{productName}/Plan/{planName}/[controller]")]
        public async Task<IActionResult> GetPublishedPlanFeaturesListByPlanNameAsync([FromRoute] string productOwnerName, string productName, [FromRoute] string planName, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planFeatureService.GetPublishedPlanFeaturesListByPlanNameAsync(productOwnerName, productName, planName, cancellationToken));
        }

        #endregion
    }
}

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

        [HttpGet("Product/{name}/[controller]")]
        public async Task<IActionResult> GetPlanFeaturesListByProductNameAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planFeatureService.GetPublishedPlanFeaturesListByProductNameAsync(name, cancellationToken));
        }

        [HttpGet("Product/{productName}/Plan/{name}/[controller]")]
        public async Task<IActionResult> GetPublishedPlanFeaturesListByPlanNameAsync([FromRoute] string productName, [FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planFeatureService.GetPublishedPlanFeaturesListByPlanNameAsync(productName, name, cancellationToken));
        }

        #endregion
    }
}

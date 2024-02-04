using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Plans;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class PlansController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<PlansController> _logger;
        private readonly IPlanService _planService;
        #endregion


        #region Corts
        public PlansController(ILogger<PlansController> logger,
                                      IPlanService planService)
        {
            _logger = logger;
            _planService = planService;
        }
        #endregion


        #region Actions    

        [HttpGet("Product/{name}/[controller]")]
        public async Task<IActionResult> GetPublishedPlansListByProductNameAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planService.GetPublishedPlansListByProductNameAsync(name, cancellationToken));
        }

        #endregion
    }
}

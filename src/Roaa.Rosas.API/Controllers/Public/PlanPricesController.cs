using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.PlanPrices;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class PlanPricesController : BaseRosasPublicApiController
    {
        #region Props 
        private readonly ILogger<PlanPricesController> _logger;
        private readonly IPlanPriceService _planPriceService;
        #endregion


        #region Corts
        public PlanPricesController(ILogger<PlanPricesController> logger,
                                      IPlanPriceService planPriceService)
        {
            _logger = logger;
            _planPriceService = planPriceService;
        }
        #endregion


        #region Actions    

        [HttpGet("Product/{name}/[controller]")]
        public async Task<IActionResult> GetPublishedPlanPricesListByProductNameAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planPriceService.GetPublishedPlanPricesListByProductNameAsync(name, cancellationToken));
        }

        [HttpGet("Product/{productName}/[controller]/{name}")]
        public async Task<IActionResult> GetPublishedPlanPricesListByProductNameAsync([FromRoute] string productName, [FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planPriceService.GetPublishedPlanPriceByPlanPriceNameAsync(productName, name, cancellationToken));
        }

        #endregion
    }
}

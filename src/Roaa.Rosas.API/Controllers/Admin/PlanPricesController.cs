using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.PlanPrices;
using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    public class PlanPricesController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<PlanPricesController> _logger;
        private readonly IPlanPriceService _planPriceService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion


        #region Corts
        public PlanPricesController(ILogger<PlanPricesController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IPlanPriceService planPriceService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _planPriceService = planPriceService;
        }
        #endregion


        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetPlanFeaturesListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _planPriceService.GetPlanPricesListByProductIdAsync(productId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreatePlanFeatureAsync([FromBody] CreatePlanPriceModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _planPriceService.CreatePlanPriceAsync(model, productId, cancellationToken));
        }


        [HttpPut("{planPriceId}")]
        public async Task<IActionResult> UpdatePlanFeatureAsync([FromBody] UpdatePlanPriceModel model, [FromRoute] Guid planPriceId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planPriceService.UpdatePlanPriceAsync(planPriceId, model, productId, cancellationToken));
        }


        [HttpDelete("{planPriceId}")]
        public async Task<IActionResult> DeletePlanFeatureAsync([FromRoute] Guid planPriceId, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _planPriceService.DeletePlanPriceAsync(planPriceId, productId, cancellationToken));
        }

        #endregion 
    }
}

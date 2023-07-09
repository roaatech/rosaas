using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Features;
using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    public class FeaturesController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<FeaturesController> _logger;
        private readonly IFeatureService _featureService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public FeaturesController(ILogger<FeaturesController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IFeatureService featureService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _featureService = featureService;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetFeaturesListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _featureService.GetFeaturesListByProductIdAsync(productId, cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeatureByIdAsync([FromRoute] Guid productId, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _featureService.GetFeatureByIdAsync(id, productId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateFeatureAsync([FromBody] CreateFeatureModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _featureService.CreateFeatureAsync(model, productId, cancellationToken));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeatureAsync([FromBody] UpdateFeatureModel model, [FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _featureService.UpdateFeatureAsync(id, model, productId, cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeatureAsync([FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _featureService.DeleteFeatureAsync(id, productId, cancellationToken));
        }

        #endregion

    }
}

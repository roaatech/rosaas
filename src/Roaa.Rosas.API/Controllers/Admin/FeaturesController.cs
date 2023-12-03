using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Features;
using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    [Authorize(Policy = AuthPolicy.Management.Features, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public partial class FeaturesController : BaseManagementApiController
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

        [HttpGet($"/{PrefixSuperAdminMainApiRoute}/[controller]")]
        public async Task<IActionResult> GetFeaturesListByProductIdAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _featureService.GetFeaturesPaginatedListAsync(pagination, filters, sort, cancellationToken));
        }


        [HttpGet("Lookup")]
        public async Task<IActionResult> GetFeaturesLookupListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _featureService.GetFeaturesLookupListByProductIdAsync(productId, cancellationToken));
        }


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

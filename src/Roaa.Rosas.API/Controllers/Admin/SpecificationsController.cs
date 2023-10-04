using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Specifications;
using Roaa.Rosas.Application.Services.Management.Specifications.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route($"{PrefixSuperAdminMainApiRoute}/products/{{productId}}/[controller]")]
    public partial class SpecificationsController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<SpecificationsController> _logger;
        private readonly ISpecificationService _featureService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public SpecificationsController(ILogger<SpecificationsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               ISpecificationService featureService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _featureService = featureService;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetSpecificationsListByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ListResult(await _featureService.GetSpecificationsListByProductIdAsync(productId, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateSpecificationAsync([FromBody] CreateSpecificationModel model, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _featureService.CreateSpecificationAsync(productId, model, cancellationToken));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecificationAsync([FromBody] UpdateSpecificationModel model, [FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _featureService.UpdateSpecificationAsync(id, productId, model, cancellationToken));
        }


        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishSpecificationAsync([FromBody] PublishSpecificationModel model, [FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _featureService.PublishSpecificationAsync(id, productId, model, cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecificationAsync([FromRoute] Guid id, [FromRoute] Guid productId, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _featureService.DeleteSpecificationAsync(id, productId, cancellationToken));
        }
        #endregion

    }


}

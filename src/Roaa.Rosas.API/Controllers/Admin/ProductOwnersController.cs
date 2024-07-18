using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.ProductOwners;
using Roaa.Rosas.Application.Services.Management.ProductOwners.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;
namespace Roaa.Rosas.API.Controllers.Admin

{
    [Route($"{PrefixSuperAdminMainApiRoute}/productowners")]
    [Authorize(Policy = AuthPolicy.Management.ProductOwners, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ProductOwnersController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<ProductOwnersController> _logger;
        private readonly IProductOwnerService _productOwnerService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public ProductOwnersController(
            ILogger<ProductOwnersController> logger,
            IWebHostEnvironment environment,
            IIdentityContextService identityContextService,
            IProductOwnerService productOwnerService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _productOwnerService = productOwnerService;
        }
        #endregion

        #region Actions   

        [HttpGet]
        public async Task<IActionResult> GetAllProductOwnersAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _productOwnerService.GetAllProductOwnersAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductOwnerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _productOwnerService.GetProductOwnerByIdAsync(id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductOwnerAsync([FromBody] CreateProductOwnerModel model, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _productOwnerService.CreateProductOwnerAsync(model, cancellationToken));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductOwnerAsync(Guid id, [FromBody] UpdateProductOwnerModel model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productOwnerService.UpdateProductOwnerAsync(id, model, cancellationToken));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductOwnerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productOwnerService.DeleteProductOwnerAsync(id, cancellationToken));
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetProductOwnersPagedAsync([FromQuery] PaginationMetaData paginationInfo, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            var result = await _productOwnerService.GetProductOwnersPaginatedListAsync(paginationInfo, filters, sort, cancellationToken);
            return PaginatedResult(result);
        }

        [HttpGet("is-registered")]
        public async Task<IActionResult> IsProductOwnerRegisteredAsync(CancellationToken cancellationToken = default)
        {
            var result = await _productOwnerService.IsProductOwnerRegisteredAsync(cancellationToken);
            return ItemResult(result);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetProductOwnerDetailsByCreatorUserIdAsync(CancellationToken cancellationToken = default)
        {
            var userId = _identityContextService.UserId;
            var result = await _productOwnerService.GetProductOwnerDetailsByCreatorUserIdAsync(userId, cancellationToken);
            return ItemResult(result);
        }



        #endregion 
    }
}


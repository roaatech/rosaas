using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Application.Services.Management.Products.Queries.GetProductWarnings;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionDetails;
using Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsListByProduct;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    [Authorize(Policy = AuthPolicy.Management.Products, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ProductsController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public ProductsController(ILogger<ProductsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IProductService productService,
                                ISender mediator)
        {
            _logger = logger;
            _productService = productService;
            _mediator = mediator;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetProductsPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _productService.GetProductsPaginatedListAsync(pagination, filters, sort, cancellationToken));
        }


        [HttpGet("Lookup")]
        public async Task<IActionResult> GetProductsLookupListAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _productService.GetProductsLookupListAsync(cancellationToken));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _productService.GetProductByIdAsync(id, cancellationToken));
        }


        [HttpPost()]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductModel model, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _productService.CreateProductAsync(model, cancellationToken));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductModel model, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productService.UpdateProductAsync(id, model, cancellationToken));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productService.DeleteProductAsync(id, cancellationToken));
        }


        [HttpGet("{id}/Tenants/{tenantId}")]
        public async Task<IActionResult> GetSubscriptionDetailsAsync([FromRoute] Guid tenantId, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetSubscriptionDetailsQuery(tenantId, id), cancellationToken));
        }


        [HttpGet("{id}/subscriptions")]
        public async Task<IActionResult> GetSubscriptionsListAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ItemResult(await _mediator.Send(new GetSubscriptionsListByProductQuery(id), cancellationToken));
        }

        [HttpGet("{id}/Warnings")]
        public async Task<IActionResult> GetProductWarningsAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetProductWarningsQuery(id), cancellationToken));
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishProductAsync([FromBody] PublishProductModel model, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productService.PublishProductAsync(id, model, cancellationToken));
        }

        [HttpPost("{id}/TrialType")]
        public async Task<IActionResult> ChangeProductTrialTypeAsync([FromBody] ChangeProductTrialTypeModel model, [FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _productService.ChangeProductTrialTypeAsync(id, model, cancellationToken));
        }
        #endregion


    }
}

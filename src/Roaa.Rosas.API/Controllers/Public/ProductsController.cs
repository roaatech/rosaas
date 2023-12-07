using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Public
{
    public class ProductsController : BaseRosasPublicApiController
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

        [HttpGet("[controller]/Lookup")]
        public async Task<IActionResult> GetProductsLookupListAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _productService.GetProductsLookupListAsync(string.Empty, cancellationToken));
        }

        [HttpGet("Client/{name}/[controller]/Lookup")]
        public async Task<IActionResult> GetProductsLookupListAsync([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return ListResult(await _productService.GetProductsLookupListAsync(name, cancellationToken));
        }
        #endregion


    }
}

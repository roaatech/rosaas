﻿using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Products;
using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class ProductsController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public ProductsController(ILogger<ProductsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                               IProductService productService)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _productService = productService;
        }
        #endregion

        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetProductsPaginatedListAsync([FromQuery] PaginationMetaData pagination, [FromQuery] List<FilterItem> filters, [FromQuery] SortItem sort, CancellationToken cancellationToken = default)
        {
            return PaginatedResult(await _productService.GetProductsPaginatedListAsync(pagination, filters, sort, cancellationToken));
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
        #endregion


    }
}

﻿using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Products
{
    public interface IProductService
    {
        Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

        Task<PaginatedResult<ProductListItemDto>> GetProductsPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateProductAsync(CreateProductModel model, Guid currentUserId, CancellationToken cancellationToken = default);

        Task<Result> UpdateProductAsync(UpdateProductModel model, CancellationToken cancellationToken = default);

        Task<Result> DeleteProductAsync(DeleteResourceModel<Guid> model, CancellationToken cancellationToken = default);
    }
}
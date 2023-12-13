using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Products
{
    public interface IProductService
    {
        Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, Expression<Func<Subscription, ProductUrlListItem>> selector, CancellationToken cancellationToken = default);

        Task<Result<T>> GetProductEndpointByIdAsync<T>(Guid productId, Expression<Func<Product, T>> selector, CancellationToken cancellationToken = default);

        Task<Result<List<Custom2LookupItemDto<Guid>>>> GetProductsLookupListAsync(string clientName, CancellationToken cancellationToken = default);

        Task<PaginatedResult<ProductListItemDto>> GetProductsPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<List<CustomLookupItemDto<Guid>>>> GetProductsLookupListAsync(CancellationToken cancellationToken = default);

        Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateProductAsync(CreateProductModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdateProductAsync(Guid id, UpdateProductModel model, CancellationToken cancellationToken = default);

        Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

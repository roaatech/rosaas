using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Products
{
    public interface IProductService
    {
        Task<Result<List<ProductUrlListItem>>> GetProductsUrlsByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    }
}

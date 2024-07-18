using Roaa.Rosas.Application.Services.Management.ProductOwners.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.ProductOwners
{
    public interface IProductOwnerService
    {
        Task<Result<List<ProductOwnerListItemDto>>> GetAllProductOwnersAsync(CancellationToken cancellationToken = default);

        Task<Result<ProductOwnerDto>> GetProductOwnerByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateProductOwnerAsync(CreateProductOwnerModel productOwner, CancellationToken cancellationToken = default);

        Task<Result> DeleteProductOwnerAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result> UpdateProductOwnerAsync(Guid id, UpdateProductOwnerModel productOwner, CancellationToken cancellationToken = default);

        Task<PaginatedResult<ProductOwnerListItemDto>> GetProductOwnersPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<CustomLookupItemDto<Guid>>> GetProductOwnerProfileByCreatorUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<ProductOwnerDto>> GetProductOwnerDetailsByCreatorUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    }
}

using Roaa.Rosas.Application.Services.Management.Features.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Features
{
    public interface IFeatureService
    {
        Task<PaginatedResult<FeatureListItemDto>> GetFeaturesPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<List<FeatureListItemDto>>> GetFeaturesListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<List<LookupItemDto<Guid>>>> GetFeaturesLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<FeatureDto>> GetFeatureByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateFeatureAsync(CreateFeatureModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> UpdateFeatureAsync(Guid id, UpdateFeatureModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeleteFeatureAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);
    }
}

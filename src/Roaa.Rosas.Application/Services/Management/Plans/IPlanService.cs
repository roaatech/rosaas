using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Plans
{
    public interface IPlanService
    {
        Task<PaginatedResult<PlanListItemDto>> GetPlansPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<List<PlanListItemDto>>> GetPlansListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<List<PlanPublishedListItemDto>>> GetPublishedPlansListByProductNameAsync(string productName, CancellationToken cancellationToken = default);

        Task<Result<List<LookupItemDto<Guid>>>> GetPlansLookupListByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<Result<PlanDto>> GetPlanByIdAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreatePlanAsync(CreatePlanModel model, Guid productId, CancellationToken cancellationToken = default, TenancyType tenancyType = TenancyType.Planed, bool isLockedBySystem = false);

        Task<Result> UpdatePlanAsync(Guid id, UpdatePlanModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> PublishPlanAsync(Guid id, PublishPlanModel model, Guid productId, CancellationToken cancellationToken = default);

        Task<Result> DeletePlanAsync(Guid id, Guid productId, CancellationToken cancellationToken = default);
    }
}

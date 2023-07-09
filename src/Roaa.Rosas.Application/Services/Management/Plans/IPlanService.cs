using Roaa.Rosas.Application.Services.Management.Plans.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Plans
{
    public interface IPlanService
    {
        Task<PaginatedResult<PlanListItemDto>> GetPlansPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<PlanDto>> GetPlanByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreatePlanAsync(CreatePlanModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdatePlanAsync(Guid id, UpdatePlanModel model, CancellationToken cancellationToken = default);

        Task<Result> DeletePlanAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

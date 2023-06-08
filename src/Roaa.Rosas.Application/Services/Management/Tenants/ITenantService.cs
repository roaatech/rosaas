using Roaa.Rosas.Application.Services.Management.Tenants.Models;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public interface ITenantService
    {
        Task<PaginatedResult<TenantListItemDto>> GetTenantsPaginatedListAsync(PaginationMetaData paginationInfo, List<FilterItem> filters, SortItem sort, CancellationToken cancellationToken = default);

        Task<Result<TenantDto>> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<TenantStatusDto>> GetTenantStatusByIdAsync(TenantStatusModel model, CancellationToken cancellationToken = default);

        Task<Result<CreatedResult<Guid>>> CreateTenantAsync(CreateTenantModel model, Guid currentUserId, CancellationToken cancellationToken = default);

        Task<Result> UpdateTenantAsync(UpdateTenantModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdateTenantStatusAsync(UpdateTenantStatusModel model, CancellationToken cancellationToken = default);

        Task<Result> DeleteTenantAsync(DeleteResourceModel<Guid> model, CancellationToken cancellationToken = default);
    }
}
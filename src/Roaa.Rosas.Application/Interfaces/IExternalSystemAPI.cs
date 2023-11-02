using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Models.ExternalSystems;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IExternalSystemAPI
    {
        Task<Result<ExternalSystemResultModel<dynamic>>> CreateTenantAsync(ExternalSystemRequestModel<CreateTenantModel> model, CancellationToken cancellationToken = default);
        Task<Result<ExternalSystemResultModel<dynamic>>> ActivateTenantAsync(ExternalSystemRequestModel<ActivateTenantModel> model, CancellationToken cancellationToken = default);
        Task<Result<ExternalSystemResultModel<dynamic>>> DeactivateTenantAsync(ExternalSystemRequestModel<DeactivateTenantModel> model, CancellationToken cancellationToken = default);
        Task<Result<ExternalSystemResultModel<dynamic>>> DeleteTenantAsync(ExternalSystemRequestModel<DeleteTenantModel> model, CancellationToken cancellationToken = default);

        Task<Result<ExternalSystemResultModel<dynamic>>> InformTheTenantUnavailableAsync(ExternalSystemRequestModel<InformTheTenantUnavailableModel> model, CancellationToken cancellationToken = default);
        Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(ExternalSystemRequestModel<CheckTenantHealthStatusModel> model, CancellationToken cancellationToken = default);
    }


}

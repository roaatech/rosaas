using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Service
{
    public interface ITenantService
    {
        Task<Result<ChangeTenantStatusResult>> ChangeTenantStatusAsync(ChangeTenantStatusModel model, CancellationToken cancellationToken = default);
    }
}
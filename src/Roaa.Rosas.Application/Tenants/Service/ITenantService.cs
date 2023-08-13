using Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Tenants.Service.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Tenants.Service
{
    public interface ITenantService
    {
        Task<Result<T>> GetByIdAsync<T>(Guid tenantId, Expression<Func<Tenant, T>> selector, CancellationToken cancellationToken = default);

        Task<Result<List<TenantStatusChangedResultDto>>> ChangeTenantStatusAsync(ChangeTenantStatusModel model, CancellationToken cancellationToken);

        Task<Result<List<SetTenantNextStatusResult>>> SetTenantNextStatusAsync(SetTenantNextStatusModel model, CancellationToken cancellationToken = default);
    }
}
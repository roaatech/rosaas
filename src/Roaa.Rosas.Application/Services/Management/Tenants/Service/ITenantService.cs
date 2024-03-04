using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;
using Roaa.Rosas.Application.Services.Management.Tenants.Service.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using System.Linq.Expressions;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service
{
    public interface ITenantService
    {
        Task<Result<T>> GetByIdAsync<T>(Guid tenantId, Expression<Func<Tenant, T>> selector, CancellationToken cancellationToken = default);

        Task<Result<List<TenantStatusChangedResultDto>>> SetTenantNextStatusAsync(Guid tenantId,
                                                                                  TenantStatus status,
                                                                                  Guid? productId,
                                                                                  WorkflowAction action,
                                                                                  ExpectedTenantResourceStatus? expectedResourceStatus,
                                                                                  string comment,
                                                                                  dynamic? receivedRequestBody,
                                                                                  CancellationToken cancellationToken = default);

        Task<Result<List<SetTenantNextStatusResult>>> SetTenantNextStatusAsync(SetTenantNextStatusModel model, CancellationToken cancellationToken = default);

        List<TenantSystemName> BuildTenantSystemNameEntities(string systemName, List<Guid> productIdS, Guid tenantCreationRequestId, Guid? tenantId = null);



    }
}
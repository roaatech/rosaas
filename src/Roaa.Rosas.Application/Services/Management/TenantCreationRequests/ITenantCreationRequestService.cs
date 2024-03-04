using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.TenantCreationRequests
{
    public interface ITenantCreationRequestService
    {
        Task<Result<List<TenantCreationPreparationModel>>> PrepareTenantCreationAsync(TenantCreationRequestModel request, Guid? tenantCreationRequestId, CancellationToken cancellationToken = default);

        TenantCreationRequest BuildTenantCreationRequestEntity(Guid orderId, string systemName, string displayName, List<TenantCreationRequestSpecification> specifications);
    }
}

using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Order BuildOrderEntity(string tenantName, string tenantDisplayName, List<TenantCreationPreparationModel> plansDataList);

    }
}

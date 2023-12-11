using Roaa.Rosas.Application.Services.Management.PlanPrices.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Orders
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}

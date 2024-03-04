using Roaa.Rosas.Application.Services.Management.Orders.Models;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Orders
{
    public interface IOrderService
    {
        Task<List<KeyValuePair<Guid, PaymentMethodCardDto>>> GetPaymentMethodCardsListAsync(List<Guid?> subscriptions, CancellationToken cancellationToken = default);

        Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Result<OrderDto>> GetOrderByIdForAnonymousAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Result<List<OrderDto>>> GetOrdersListAsync(Guid tenantId, CancellationToken cancellationToken = default);

        Order BuildOrderEntity(string tenantName, string tenantDisplayName, List<TenantCreationPreparationModel> plansDataList);

        Task MarkOrderAsUpgradingFromTrialToRegularSubscriptionAsync(Order order, CancellationToken cancellationToken = default);

        Task<Result> ChangeOrderPlanAsync(Guid orderId, ChangeOrderPlanModel model, CancellationToken cancellationToken = default);

        Task SetSubscriptionIdToOrderItemsAsync(Guid orderId, Guid tenantId, List<Subscription> subscriptions, CancellationToken cancellationToken);
    }
}

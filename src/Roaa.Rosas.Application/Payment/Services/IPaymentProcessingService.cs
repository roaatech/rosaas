using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Services
{
    public interface IPaymentProcessingService
    {
        Task<Order> MarkOrderAsProcessingAsync(Order order, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsAuthorizedAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsAuthorizedAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsPaidAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsFailedAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsFailedAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}




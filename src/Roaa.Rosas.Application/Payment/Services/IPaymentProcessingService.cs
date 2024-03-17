using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Services
{
    public interface IPaymentProcessingService
    {
        Task<Order> MarkOrderAsProcessingAsync(Order order, PaymentPlatform paymentPlatform, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsAuthorizedAsync(Order order, string cardReferenceId, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsAuthorizedAsync(Guid orderId, string cardReferenceId, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsPaidAsync(Order order, string cardReferenceId, PaymentPlatform paymentPlatform, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsPaidAsync(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsFailedAsync(Order order, CancellationToken cancellationToken = default);

        Task<Order> MarkOrderAsFailedAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}




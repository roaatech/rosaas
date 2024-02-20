using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Methods
{
    public interface IPaymentMethodService
    {
        Task<Result<PaymentMethodCheckoutResultModel>> CreatePaymentAsync(Order order, bool setAuthorizedPayment, CancellationToken cancellationToken = default);

        Task<Result<Order>> CompleteSuccessfulPaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Result> CapturePaymentAsync(Order order, CancellationToken cancellationToken = default);

        PaymentMethodType PaymentMethodType { get; }
    }

}

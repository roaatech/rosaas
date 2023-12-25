using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Methods
{
    public interface IPaymentMethodService
    {
        Task<Result<CheckoutResultModel>> HandelPaymentProcessAsync(Order order, CancellationToken cancellationToken = default);

        Task<Result<Order>> CompletePaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<DateTime> GetPaymentProcessingExpirationDate(CancellationToken cancellationToken = default);

        PaymentMethodType PaymentMethodType { get; }
    }

}

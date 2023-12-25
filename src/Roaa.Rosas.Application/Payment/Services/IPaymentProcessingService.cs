using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Services
{
    public interface IPaymentProcessingService
    {
        Task MarkOrderAsProcessingAsync(Order order, PaymentMethodType? paymentMethodType, CancellationToken cancellationToken = default);

        Task<Result<CheckoutResultModel>> MarkOrderAsPaidAsync(Order order, CancellationToken cancellationToken = default);
    }
}




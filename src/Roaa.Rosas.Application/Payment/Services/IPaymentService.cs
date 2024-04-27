using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Payment.Services
{
    public interface IPaymentService
    {
        Task<Result<CheckoutResultModel>> CheckoutAsync(CheckoutModel model, CancellationToken cancellationToken = default);

        Task<Result> CapturePaymentAsync(Guid orderId, PaymentPurpose paymentPurpose, CancellationToken cancellationToken = default);

        Task<Result> PayAsync(Guid orderId, string referenceCardId, PaymentPurpose paymentPurpose, Guid userId, UserType userType, CancellationToken cancellationToken = default);

        Task<Result> DoRecurringPaymentAsync(Order order, string referenceCardId, PaymentPurpose paymentPurpose, Guid userId, UserType userType, CancellationToken cancellationToken = default);
    }





}




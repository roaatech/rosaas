using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Payment.Platforms
{
    public interface IPaymentPlatformService
    {
        Task<Result<PaymentMethodCheckoutResultModel>> CreatePaymentAsync(Order order, bool setAuthorizedPayment, bool storeCardInfo, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default);

        Task<Result<Order>> CompleteSuccessfulPaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<Result> CapturePaymentAsync(Order order, PaymentPurpose paymentPurpose, CancellationToken cancellationToken = default);

        Task<Result> DoRecurringPaymentAsync(Order order, string referenceCardId, PaymentPurpose paymentPurpose, Guid userId, UserType userType, CancellationToken cancellationToken = default);

        PaymentPlatform PaymentPlatform { get; }
    }

}

using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Payment.Methods.StripeService
{
    public interface IStripePaymentMethodService
    {
        Task<Result<CheckoutResultModel>> SuccessAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default);

        Task<Result<CheckoutResultModel>> CancelAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default);


    }
}







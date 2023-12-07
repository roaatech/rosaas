using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application
{
    public interface IPaymentMethod
    {
        Task<Result<CheckoutResultModel>> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default);
        Task<Result<CheckoutResultModel>> CancelAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default);
        Task<Result<CheckoutResultModel>> SuccessAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default);
    }

}

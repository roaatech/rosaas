using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment
{
    public interface IPaymentMethod
    {
        Task<Result<CheckoutResultModel>> DoProcessPaymentAsync(Order order, CancellationToken cancellationToken = default);
        Task<Result<CheckoutResultModel>> CancelAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default);
        Task<Result<CheckoutResultModel>> SuccessAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default);
    }

}

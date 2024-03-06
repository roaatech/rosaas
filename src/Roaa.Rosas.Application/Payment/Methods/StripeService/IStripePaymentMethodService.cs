using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Models.Payment;

namespace Roaa.Rosas.Application.Payment.Methods.StripeService
{
    public interface IStripePaymentMethodService
    {
        Task<Result<CheckoutResultModel>> CompleteSuccessfulSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default);

        Task<Result<CheckoutResultModel>> CompleteFailedSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default);

        Task UpdateCustomerAsync(string name, string phone, Guid userId, CancellationToken cancellationToken = default);

        Task<Result<List<PaymentMethodCardListItem>>> GetPaymentMethodsCardsListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Result> AttachPaymentMethodCardAsync(Guid userId, string stripeCardId, CancellationToken cancellationToken = default);

        Task<Result> DetachPaymentMethodCardAsync(string stripeCardId, CancellationToken cancellationToken = default);

        Task<Result> MarkPaymentMethodAsDefaultAsync(Guid userId, string stripeCardId, CancellationToken cancellationToken = default);
    }
}







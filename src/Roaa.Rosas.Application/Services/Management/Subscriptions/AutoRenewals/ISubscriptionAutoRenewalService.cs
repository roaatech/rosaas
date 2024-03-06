using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals
{
    public interface ISubscriptionAutoRenewalService
    {
        Task<Result> EnableAutoRenewalAsync(Guid subscriptionId, string cardReferenceId, PaymentPlatform paymentPlatform, Guid? planPriceId, string? comment, CancellationToken cancellationToken = default);
        Task<Result> CancelAutoRenewalAsync(Guid subscriptionId, string? comment, CancellationToken cancellationToken);

    }
}

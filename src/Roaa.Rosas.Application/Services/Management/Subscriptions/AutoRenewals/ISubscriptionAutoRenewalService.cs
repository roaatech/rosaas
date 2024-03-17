using Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals
{
    public interface ISubscriptionAutoRenewalService
    {
        Task<Result<List<SubscriptionAutoRenewalDto>>> GetSubscriptionAutoRenewalsListByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Result> EnableAutoRenewalAsync(Guid subscriptionId,
                                            string cardReferenceId,
                                            PaymentPlatform paymentPlatform,
                                            Guid? planPriceId,
                                            string? comment,
                                            Guid userId,
                                            CancellationToken cancellationToken = default);
        Task<Result> CancelAutoRenewalAsync(Guid subscriptionId, string? comment, CancellationToken cancellationToken);

    }
}

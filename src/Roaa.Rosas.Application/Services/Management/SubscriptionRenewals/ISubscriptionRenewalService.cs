using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals
{
    public interface ISubscriptionRenewalService
    {
        Task<Result<List<SubscriptionRenewalDto>>> GetSubscriptionRenewalsListByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Result> EnableAutoRenewalAsync(Guid subscriptionId,
                                                    string cardReferenceId,
                                                    PaymentPlatform paymentPlatform,
                                                    Guid? planPriceId,
                                                    string? comment,
                                                    Guid userId,
                                                    UserType payerUsertype,
                                                    int renewalsCount,
                                                    bool isContinuousRenewal,
                                                    CancellationToken cancellationToken = default);

        Task<Result> EnableSubscriptionUpgradingAsync(Guid subscriptionId,
                                                      Guid planId,
                                                      Guid planPriceId,
                                                      string cardReferenceId,
                                                      PaymentPlatform paymentPlatform,
                                                      string? comment,
                                                      CancellationToken cancellationToken = default);


        Task<Result> EnableSubscriptionDowngradingAsync(Guid subscriptionId,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string cardReferenceId,
                                                        PaymentPlatform paymentPlatform,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default);

        Task<Result> EnableSubscriptionDowngradingAsync(Subscription subscription,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default);


        Task<Result> CancelRenewalAsync(Guid renewalId, Guid subscriptionId, string? comment, CancellationToken cancellationToken);

    }
}

using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionPlansChanging
{
    public interface ISubscriptionPlanChangingService
    {
        Task<Result> CreateSubscriptionUpgradeAsync(Guid subscriptionId,
                                                                Guid planId,
                                                                Guid planPriceId,
                                                                string cardReferenceId,
                                                                PaymentPlatform paymentPlatform,
                                                                string? comment,
                                                                CancellationToken cancellationToken = default);
        Task<Result> CreateSubscriptionDowngradeAsync(Guid subscriptionId,
                                                        Guid planId,
                                                        Guid planPriceId,
                                                        string cardReferenceId,
                                                        PaymentPlatform paymentPlatform,
                                                        string? comment,
                                                        CancellationToken cancellationToken = default);

    }
}

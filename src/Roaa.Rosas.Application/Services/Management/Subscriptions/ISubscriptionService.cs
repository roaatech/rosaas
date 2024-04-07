using Roaa.Rosas.Application.Services.Management.Subscriptions.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public interface ISubscriptionService
    {
        Task<Result<List<SubscriptionFeatureDto>>> GetSubscriptionFeaturesAsync(Guid subscriptionId, CancellationToken cancellationToken);

        Task<Result<SubscriptionDetailsDto>> GetSubscriptionDetailsAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken);

        Task<Result<List<MySubscriptionListItemDto>>> GetSubscriptionsListByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<Result<List<SubscriptionListItemDto>>> GetSubscriptionsListByProductIdAsync(Guid productId, CancellationToken cancellationToken);

        Task<Result> ResetSubscriptionPlanAsync(Subscription subscription,
                                                Guid planId,
                                                Guid planPriceId,
                                                CancellationToken cancellationToken = default);

        Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default);

        Task<Result> ResetSubscriptionsFeaturesAsync(List<SubscriptionFeature> subscriptionFeatures, string? comment, string? systemComment, CancellationToken cancellationToken = default);

        Task RenewSubscriptionAsync(Subscription subscription, SubscriptionRenewal subscriptionRenewal, bool keepCurrentSubscriptionFeatures, CancellationToken cancellationToken = default);

        Task<List<PlanFeatureInfoModel>> FetchSubscriptionPlanFeaturesAsync(Subscription subscription,
                                                                                                SubscriptionRenewal subscriptionRenewal,
                                                                                                CancellationToken cancellationToken = default);

        Task<Result> ActivateSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default);

        Task<Result> SuspendSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default);
    }
}

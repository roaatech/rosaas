using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public interface ISubscriptionService
    {
        Task<Result> TryToExtendOrSuspendSubscriptionsAsync(CancellationToken cancellationToken = default);

        Task<Result> DeactivateSubscriptionDueToNonPaymentAsync(int periodTimeAfterEndDateInHours, CancellationToken cancellationToken = default);

        Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default);

        Task<Result> ResetSubscriptionsFeaturesAsync(List<SubscriptionFeature> subscriptionFeatures, string? comment, string? systemComment, CancellationToken cancellationToken = default);

        Task<Result> ChangeSubscriptionPlanAsync(Subscription subscription, CancellationToken cancellationToken = default);

        Task<Result> Temp__RenewSubscriptionsAsync(Guid subscriptionId, CancellationToken cancellationToken = default);

        Task<Result> Temp__EndSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);

    }
}

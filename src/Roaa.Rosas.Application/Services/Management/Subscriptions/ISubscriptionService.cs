using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions
{
    public interface ISubscriptionService
    {
        Task<Result> SuspendPaymentStatusForSubscriptionDueToNonRenewalAsync(CancellationToken cancellationToken = default);

        Task<Result> DeactivateSubscriptionDueToNonPaymentAsync(int periodTimeAfterEndDateInHours, CancellationToken cancellationToken = default);

        Task<Result> ResetSubscriptionsFeaturesAsync(CancellationToken cancellationToken = default);

    }
}

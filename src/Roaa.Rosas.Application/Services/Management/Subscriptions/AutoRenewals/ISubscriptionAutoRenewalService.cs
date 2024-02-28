using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.AutoRenewals
{
    public interface ISubscriptionAutoRenewalService
    {
        Task<Result> EnableAutoRenewalAsync(Guid subscriptionId, Guid planPriceId, string comment, CancellationToken cancellationToken = default);

    }
}

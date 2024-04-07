using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals
{
    public class SubscriptionRenewalUtilities
    {
        public SubscriptionRenewalStatus[] AllowedSubscriptionRenewalStatuses { get; set; } = new[] { SubscriptionRenewalStatus.None,
                                                                              SubscriptionRenewalStatus.FailedPayment,
                                                                              SubscriptionRenewalStatus.Failure };
        public SubscriptionRenewalStatus[] AllowedRenewalCancellationStatuses { get; set; } = new[] { SubscriptionRenewalStatus.None };

        public SubscriptionRenewalStatus[] AllowedSubscriptionRenewalStatusForForcedDowngrade { get; set; } = new[] {
                                                                              SubscriptionRenewalStatus.FailedPayment,
                                                                              SubscriptionRenewalStatus.Failure };



        public bool EnsureAllowedSubscriptionRenewalStatus(SubscriptionRenewalStatus status)
        {
            return AllowedSubscriptionRenewalStatuses.Contains(status);
        }

        public bool EnsurethatTheSubscriptionRenewalStatusIsAllowedForForcedDowngrade(SubscriptionRenewalStatus status)
        {
            return AllowedSubscriptionRenewalStatusForForcedDowngrade.Contains(status);
        }

        public bool EnsureAllowedRenewalCancellationStatuses(SubscriptionRenewalStatus status)
        {
            return AllowedRenewalCancellationStatuses.Contains(status);
        }

        public bool EnsureIsForcedDowngrade(SubscriptionRenewal subscriptionRenewal)
        {
            return !(subscriptionRenewal.IsForced && subscriptionRenewal.Type == SubscriptionRenewalTypeEnum.Downgrade);
        }
    }
}

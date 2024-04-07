using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionTrials
{
    public class TrialSubscriptionUtilities
    {
        public SubscriptionTrialStatus[] AllowedTrialSubscriptionStatuses { get; set; } = new[] {
                                                                        SubscriptionTrialStatus.None,
                                                                        SubscriptionTrialStatus.FailedPayment,
                                                                        SubscriptionTrialStatus.Failure };

        public SubscriptionTrialStatus[] AllowedTrialSubscriptionStatusesForForcedDowngrade { get; set; } = new[] {
                                                                        SubscriptionTrialStatus.FailedPayment,
                                                                        SubscriptionTrialStatus.Failure };

        public bool EnsurethatTheTrialSubscriptionStatusIsAllowedForForcedDowngrade(SubscriptionTrialStatus status)
        {
            return AllowedTrialSubscriptionStatusesForForcedDowngrade.Contains(status);
        }
        public bool EnsurethatTheTrialSubscriptionStatusIsAllowedForUpgrade(SubscriptionTrialStatus status)
        {
            return AllowedTrialSubscriptionStatuses.Contains(status);
        }
    }
}

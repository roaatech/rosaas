using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.Upgrade)]
    public class SubscriptionUpgradingDisabledEvent : SubscriptionRenewalHasBeenDisabledBaseEvent
    {
        public SubscriptionUpgradingDisabledEvent(SubscriptionRenewal subscriptionRenewal)
            : base(subscriptionRenewal)
        {
        }

        public SubscriptionUpgradingDisabledEvent()
        {
        }
    }
}

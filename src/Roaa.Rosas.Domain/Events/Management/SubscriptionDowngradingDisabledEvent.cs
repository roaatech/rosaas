using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.Downgrade)]
    public class SubscriptionDowngradingDisabledEvent : SubscriptionRenewalHasBeenDisabledBaseEvent
    {
        public SubscriptionDowngradingDisabledEvent(SubscriptionRenewal subscriptionRenewal)
            : base(subscriptionRenewal)
        {
        }

        public SubscriptionDowngradingDisabledEvent()
        {
        }
    }
}

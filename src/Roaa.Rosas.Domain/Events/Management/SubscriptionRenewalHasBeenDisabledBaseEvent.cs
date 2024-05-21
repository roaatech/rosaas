using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{

    public abstract class SubscriptionRenewalHasBeenDisabledBaseEvent : BaseInternalEvent
    {
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionRenewalHasBeenDisabledBaseEvent()
        {
        }

        public SubscriptionRenewalHasBeenDisabledBaseEvent(SubscriptionRenewal subscriptionRenewal)
        {
            SubscriptionRenewal = subscriptionRenewal;

        }
    }
    public class SubscriptionRenewalHasBeenDisabledEvent : BaseInternalEvent
    {
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionRenewalHasBeenDisabledEvent()
        {
        }

        public SubscriptionRenewalHasBeenDisabledEvent(SubscriptionRenewal subscriptionRenewal)
        {
            SubscriptionRenewal = subscriptionRenewal;

        }
    }
}

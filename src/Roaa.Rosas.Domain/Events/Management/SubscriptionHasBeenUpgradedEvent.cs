using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionHasBeenUpgradedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionHasBeenUpgradedEvent()
        {
        }

        public SubscriptionHasBeenUpgradedEvent(Subscription subscription,
                                        SubscriptionRenewal subscriptionRenewal)
        {
            Subscription = subscription;
            SubscriptionRenewal = subscriptionRenewal;
        }
    }
}

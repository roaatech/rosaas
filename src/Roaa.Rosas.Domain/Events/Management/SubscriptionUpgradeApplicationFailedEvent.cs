using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionUpgradeApplicationFailedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionUpgradeApplicationFailedEvent()
        {
        }

        public SubscriptionUpgradeApplicationFailedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

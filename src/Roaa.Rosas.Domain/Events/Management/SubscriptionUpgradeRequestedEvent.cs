using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionUpgradeRequestedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionUpgradeRequestedEvent()
        {
        }

        public SubscriptionUpgradeRequestedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

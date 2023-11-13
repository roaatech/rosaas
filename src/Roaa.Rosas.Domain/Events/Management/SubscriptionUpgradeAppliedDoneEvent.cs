using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionUpgradeAppliedDoneEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionUpgradeAppliedDoneEvent()
        {
        }

        public SubscriptionUpgradeAppliedDoneEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionDowngradeRequestedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionDowngradeRequestedEvent()
        {
        }

        public SubscriptionDowngradeRequestedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionDowngradeApplicationFailedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionDowngradeApplicationFailedEvent()
        {
        }

        public SubscriptionDowngradeApplicationFailedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

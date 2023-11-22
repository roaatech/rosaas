using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionDowngradeAppliedDoneEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionDowngradeAppliedDoneEvent()
        {
        }

        public SubscriptionDowngradeAppliedDoneEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

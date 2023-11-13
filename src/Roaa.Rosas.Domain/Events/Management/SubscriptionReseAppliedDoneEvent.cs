using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionReseAppliedDoneEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionReseAppliedDoneEvent()
        {
        }

        public SubscriptionReseAppliedDoneEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

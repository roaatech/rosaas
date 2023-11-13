using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionResetFailureEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionResetFailureEvent()
        {
        }

        public SubscriptionResetFailureEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

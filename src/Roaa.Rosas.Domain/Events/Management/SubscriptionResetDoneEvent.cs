using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionResetDoneEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionResetDoneEvent()
        {
        }

        public SubscriptionResetDoneEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

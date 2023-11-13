using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionResetApplicationFailedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionResetApplicationFailedEvent()
        {
        }

        public SubscriptionResetApplicationFailedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

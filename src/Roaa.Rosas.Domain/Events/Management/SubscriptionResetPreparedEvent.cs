using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionResetPreparedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionResetPreparedEvent()
        {
        }

        public SubscriptionResetPreparedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionDowngradePreparedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionDowngradePreparedEvent()
        {
        }

        public SubscriptionDowngradePreparedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

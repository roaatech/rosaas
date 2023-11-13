using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionUpgradePreparedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();

        public SubscriptionUpgradePreparedEvent()
        {
        }

        public SubscriptionUpgradePreparedEvent(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

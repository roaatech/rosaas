using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionPlanDowngradedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionPlanChanging SubscriptionPlanChange { get; set; } = new();
        public SubscriptionCycle PreviousSubscriptionCycle { get; set; } = new();

        public SubscriptionPlanDowngradedEvent()
        {
        }

        public SubscriptionPlanDowngradedEvent(Subscription subscription,
                                                    SubscriptionPlanChanging subscriptionPlanChange,
                                                    SubscriptionCycle previousSubscriptionCycle)
        {
            Subscription = subscription;
            SubscriptionPlanChange = subscriptionPlanChange;
            PreviousSubscriptionCycle = previousSubscriptionCycle;
        }
    }
}

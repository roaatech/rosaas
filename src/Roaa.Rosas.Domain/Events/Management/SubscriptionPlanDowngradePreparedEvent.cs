using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionPlanDowngradePreparedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionPlanChanging SubscriptionPlanChange { get; set; } = new();
        public SubscriptionCycle PreviousSubscriptionCycle { get; set; } = new();

        public SubscriptionPlanDowngradePreparedEvent()
        {
        }

        public SubscriptionPlanDowngradePreparedEvent(Subscription subscription,
                                                    SubscriptionPlanChanging subscriptionPlanChange,
                                                    SubscriptionCycle previousSubscriptionCycle)
        {
            Subscription = subscription;
            SubscriptionPlanChange = subscriptionPlanChange;
            PreviousSubscriptionCycle = previousSubscriptionCycle;
        }
    }
}

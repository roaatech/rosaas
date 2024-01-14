using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionPlanChangePreparedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionPlanChanging SubscriptionPlanChange { get; set; } = new();
        public Guid PreviousSubscriptionCycleId { get; set; }



        public SubscriptionPlanChangePreparedEvent()
        {
        }


        public SubscriptionPlanChangePreparedEvent(Subscription subscription,
                                        SubscriptionPlanChanging subscriptionPlanChange,
                                        Guid previousSubscriptionCycleId)
        {
            Subscription = subscription;
            SubscriptionPlanChange = subscriptionPlanChange;
            PreviousSubscriptionCycleId = previousSubscriptionCycleId;
        }
    }
}

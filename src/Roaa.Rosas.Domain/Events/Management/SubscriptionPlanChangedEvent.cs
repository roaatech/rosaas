using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionPlanChangedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionPlanChanging SubscriptionPlanChange { get; set; } = new();
        public Guid PreviousSubscriptionCycleId { get; set; }



        public SubscriptionPlanChangedEvent()
        {
        }

        public SubscriptionPlanChangedEvent(Subscription subscription,
                                        SubscriptionPlanChanging subscriptionPlanChange,
                                        Guid previousSubscriptionCycleId)
        {
            Subscription = subscription;
            SubscriptionPlanChange = subscriptionPlanChange;
            PreviousSubscriptionCycleId = previousSubscriptionCycleId;
        }
    }
    //public class SubscriptionPlanChangeAppliedEvent : BaseInternalEvent
    //{
    //    public Subscription Subscription { get; set; } = new();
    //    public SubscriptionPlanChanging SubscriptionPlanChange { get; set; } = new();
    //    public Guid PreviousSubscriptionCycleId { get; set; }



    //    public SubscriptionPlanChangeAppliedEvent()
    //    {
    //    }

    //    public SubscriptionPlanChangeAppliedEvent(Subscription subscription,
    //                                    SubscriptionPlanChanging subscriptionPlanChange,
    //                                    Guid previousSubscriptionCycleId)
    //    {
    //        Subscription = subscription;
    //        SubscriptionPlanChange = subscriptionPlanChange;
    //        PreviousSubscriptionCycleId = previousSubscriptionCycleId;
    //    }
    //}
}

using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderCompletionAchievedForTenantCreationEvent : OrderCompletionAchievedBaseEvent
    {
        public OrderCompletionAchievedForTenantCreationEvent() : base()
        {
        }

        public OrderCompletionAchievedForTenantCreationEvent(Guid orderId)
            : base(orderId)
        {
        }
    }

    public class OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent : OrderCompletionAchievedBaseEvent
    {
        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent() : base()
        {
        }

        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent(Guid orderId)
            : base(orderId)
        {
        }
    }


    public class OrderCompletionAchievedBaseEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }

        public OrderCompletionAchievedBaseEvent()
        {
        }

        public OrderCompletionAchievedBaseEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}

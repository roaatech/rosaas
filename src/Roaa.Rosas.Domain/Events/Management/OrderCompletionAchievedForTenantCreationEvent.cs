using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderCompletionAchievedForTenantCreationEvent : OrderCompletionAchievedBaseEvent
    {
        public OrderCompletionAchievedForTenantCreationEvent() : base()
        {
        }

        public OrderCompletionAchievedForTenantCreationEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
            : base(orderId, cardReferenceId, paymentPlatform)
        {
        }
    }

    public class OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent : OrderCompletionAchievedBaseEvent
    {
        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent() : base()
        {
        }

        public OrderCompletionAchievedForUpgradingFromTrialToRegularSubscriptionEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
            : base(orderId, cardReferenceId, paymentPlatform)
        {
        }
    }


    public class OrderCompletionAchievedBaseEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }

        public string CardReferenceId { get; set; }
        public PaymentPlatform PaymentPlatform { get; set; }

        public OrderCompletionAchievedBaseEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
        {
            OrderId = orderId;
            CardReferenceId = cardReferenceId;
            PaymentPlatform = paymentPlatform;
        }
        public OrderCompletionAchievedBaseEvent()
        {
        }

    }
}

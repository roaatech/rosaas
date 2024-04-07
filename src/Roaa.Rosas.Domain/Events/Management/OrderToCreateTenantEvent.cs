using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderToCreateTenantEvent : OrderCompletedBaseEvent
    {
        public OrderToCreateTenantEvent() : base()
        {
        }

        public OrderToCreateTenantEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
            : base(orderId, cardReferenceId, paymentPlatform)
        {
        }
    }
     public class OrderAuthorizedToCreateTenantEvent : OrderCompletedBaseEvent
    {
        public OrderAuthorizedToCreateTenantEvent() : base()
        {
        }

        public OrderAuthorizedToCreateTenantEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
            : base(orderId, cardReferenceId, paymentPlatform)
        {
        }
    }

    public class OrderPaidToUpgradeTrialSubscriptionToPaidEvent : OrderCompletedBaseEvent
    {
        public OrderPaidToUpgradeTrialSubscriptionToPaidEvent() : base()
        {
        }

        public OrderPaidToUpgradeTrialSubscriptionToPaidEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
            : base(orderId, cardReferenceId, paymentPlatform)
        {
        }
    }


    public abstract class OrderCompletedBaseEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }

        public string CardReferenceId { get; set; }
        public PaymentPlatform PaymentPlatform { get; set; }

        public OrderCompletedBaseEvent(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform)
        {
            OrderId = orderId;
            CardReferenceId = cardReferenceId;
            PaymentPlatform = paymentPlatform;
        }
        public OrderCompletedBaseEvent()
        {
        }

    }
}

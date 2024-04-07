using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderPaidEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }
        public PaymentPurpose PaymentPurpose { get; set; }
        public string CardReferenceId { get; set; }
        public PaymentPlatform PaymentPlatform { get; set; }

        public OrderPaidEvent(Guid orderId, PaymentPurpose paymentPurpose, string cardReferenceId, PaymentPlatform paymentPlatform)
        {
            OrderId = orderId;
            PaymentPurpose = paymentPurpose;
            CardReferenceId = cardReferenceId;
            PaymentPlatform = paymentPlatform;
        }
    }

}

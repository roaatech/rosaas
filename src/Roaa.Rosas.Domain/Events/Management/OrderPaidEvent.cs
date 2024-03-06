using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderPaidEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }
        public OrderIntent OrderIntent { get; set; }
        public string CardReferenceId { get; set; }
        public PaymentPlatform PaymentPlatform { get; set; }

        public OrderPaidEvent(Guid orderId, OrderIntent orderIntent, string cardReferenceId, PaymentPlatform paymentPlatform)
        {
            OrderId = orderId;
            OrderIntent = orderIntent;
            CardReferenceId = cardReferenceId;
            PaymentPlatform = paymentPlatform;
        }
    }

}

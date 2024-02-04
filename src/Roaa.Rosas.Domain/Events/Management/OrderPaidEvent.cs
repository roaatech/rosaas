using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class OrderPaidEvent : BaseInternalEvent
    {
        public Guid OrderId { get; set; }
        public OrderIntent OrderIntent { get; set; }


        public OrderPaidEvent(Guid orderId, OrderIntent orderIntent)
        {
            OrderId = orderId;
            OrderIntent = orderIntent;
        }
    }

}

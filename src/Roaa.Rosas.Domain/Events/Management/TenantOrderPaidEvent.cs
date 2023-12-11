using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantOrderPaidEvent : BaseInternalEvent
    {
        public Guid TenantId { get; set; }
        public Guid OrderId { get; set; }


        public TenantOrderPaidEvent(Guid orderId, Guid tenantId)
        {
            OrderId = orderId;
            TenantId = tenantId;
        }
    }
}

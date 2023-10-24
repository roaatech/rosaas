using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantActivatedEvent : SendingRequestBaseEvent
    {
        public TenantActivatedEvent(Guid tenantId, Guid productId, Guid subscriptionId, TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
            : base(tenantId, productId, subscriptionId, status, step, previousStatus, previousStep)
        {

        }
    }
}

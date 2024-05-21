using Roaa.Rosas.Domain.Attributes;

namespace Roaa.Rosas.Domain.Events.Management
{
    [TenantAvailabilityAttribute(isHealthy: false)]
    public class TenantAvailabilityChangedToUnhealthyEvent : TenantAvailabilityChangedEvent
    {
        public TenantAvailabilityChangedToUnhealthyEvent(Guid tenantId, Guid productId)
            : base(tenantId, productId, false)
        {
        }

        public TenantAvailabilityChangedToUnhealthyEvent()
        {
        }
    }
}

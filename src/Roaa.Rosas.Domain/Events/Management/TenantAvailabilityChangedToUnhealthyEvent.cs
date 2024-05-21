using Roaa.Rosas.Domain.Attributes;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    [TenantAvailabilityAttribute(isHealthy: false)]
    public class TenantAvailabilityChangedToUnhealthyEvent : BaseInternalEvent
    {
        public TenantAvailabilityChangedToUnhealthyEvent(Guid tenantId, Guid productId)
        {
            TenantId = tenantId;
            ProductId = productId;
        }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
    }
}

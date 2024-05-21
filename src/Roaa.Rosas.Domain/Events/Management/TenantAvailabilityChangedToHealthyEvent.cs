using Roaa.Rosas.Domain.Attributes;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{

    [TenantAvailabilityAttribute(isHealthy: true)]
    public class TenantAvailabilityChangedToHealthyEvent : BaseInternalEvent
    {
        public TenantAvailabilityChangedToHealthyEvent(Guid tenantId, Guid productId)
        {
            TenantId = tenantId;
            ProductId = productId;
        }

        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
    }
}

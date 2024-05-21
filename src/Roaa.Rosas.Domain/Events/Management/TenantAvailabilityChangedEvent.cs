using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public abstract class TenantAvailabilityChangedEvent : BaseInternalEvent
    {
        public TenantAvailabilityChangedEvent(Guid tenantId, Guid productId, bool isAvailable)
        {
            TenantId = tenantId;
            ProductId = productId;
            IsAvailable = isAvailable;
        }
        public TenantAvailabilityChangedEvent()
        {
        }
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public bool IsAvailable { get; set; }
    }
}

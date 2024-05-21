namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantAvailabilityChangedToHealthyEvent : TenantAvailabilityChangedEvent
    {
        public TenantAvailabilityChangedToHealthyEvent(Guid tenantId, Guid productId)
            : base(tenantId, productId, true)
        {
        }

        public TenantAvailabilityChangedToHealthyEvent()
        {
        }
    }
}

using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class ExternalSystemIsBeingProvisionedTenantResourcesEvent : BaseInternalEvent
    {
        public ExternalSystemIsBeingProvisionedTenantResourcesEvent(Guid tenantId, Guid productId)
        {
            TenantId = tenantId;
            ProductId = productId;
        }
        public ExternalSystemIsBeingProvisionedTenantResourcesEvent()
        {
        }
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
    }
}

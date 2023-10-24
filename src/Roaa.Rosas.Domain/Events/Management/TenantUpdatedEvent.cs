using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantUpdatedEvent : BaseInternalEvent
    {
        public Tenant OldTenant { get; set; }
        public Tenant UpdatedTenant { get; set; }


        public TenantUpdatedEvent(Tenant oldTenant, Tenant updatedTenant)
        {
            OldTenant = oldTenant;
            UpdatedTenant = updatedTenant;
        }
    }
}

using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantCreatedInStoreEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus Status { get; set; }
        public TenantStep Step { get; set; }


        public TenantCreatedInStoreEvent(Tenant tenant, TenantStatus status, TenantStep step)
        {
            Tenant = tenant;
            Status = status;
            Step = step;
        }
    }
}

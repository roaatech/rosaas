using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantDeactivatedEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus OldStatus { get; set; }


        public TenantDeactivatedEvent(Tenant tenant, TenantStatus oldStatus)
        {
            Tenant = tenant;
            OldStatus = oldStatus;
        }
    }
}

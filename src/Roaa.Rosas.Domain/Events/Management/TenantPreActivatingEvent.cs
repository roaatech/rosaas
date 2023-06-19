using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantPreActivatingEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus PreviousStatus { get; set; }

        public TenantPreActivatingEvent(Tenant tenant, TenantStatus previousStatus)
        {
            Tenant = tenant;
            PreviousStatus = previousStatus;
        }
    }
}

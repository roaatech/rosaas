using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantCreatedInStoreEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }
        public TenantStatus Status { get; set; }


        public TenantCreatedInStoreEvent(Tenant tenant, TenantStatus status)
        {
            Tenant = tenant;
            Status = status;
        }
    }
}

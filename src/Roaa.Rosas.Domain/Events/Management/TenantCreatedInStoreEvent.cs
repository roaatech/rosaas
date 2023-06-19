using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantCreatedInStoreEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }


        public TenantCreatedInStoreEvent(Tenant tenant)
        {
            Tenant = tenant;
        }
    }
}

using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantCreatedEvent : BaseInternalEvent
    {
        public Tenant Tenant { get; set; }


        public TenantCreatedEvent(Tenant tenant)
        {
            Tenant = tenant;
        }
    }
}

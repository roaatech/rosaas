using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantPreActivatingEvent : BaseInternalEvent
    {
        public Subscription ProductTenant { get; set; }
        public TenantStatus PreviousStatus { get; set; }

        public TenantPreActivatingEvent(Subscription productTenant, TenantStatus previousStatus)
        {
            ProductTenant = productTenant;
            PreviousStatus = previousStatus;
        }
    }
}

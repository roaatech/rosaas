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
        public ExpectedTenantResourceStatus ExpectedResourceStatus { get; set; }
        public TenancyType TenancyType { get; set; }


        public TenantCreatedInStoreEvent(Tenant tenant, ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus status, TenantStep step, TenancyType tenancyType)
        {
            Tenant = tenant;
            Status = status;
            Step = step;
            ExpectedResourceStatus = expectedResourceStatus;
            TenancyType = tenancyType;
        }
    }
}

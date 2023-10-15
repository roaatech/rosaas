using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantPreCreatingEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }
        public TenantStatus PreviousStatus { get; set; }

        public TenantPreCreatingEvent(Subscription Subscription, TenantStatus previousStatus)
        {
            this.Subscription = Subscription;
            PreviousStatus = previousStatus;
        }
    }
}

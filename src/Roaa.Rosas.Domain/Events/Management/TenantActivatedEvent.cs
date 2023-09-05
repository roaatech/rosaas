using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantActivatedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }
        public TenantStatus PreviousStatus { get; set; }

        public TenantActivatedEvent(Subscription subscription, TenantStatus previousStatus)
        {
            Subscription = subscription;
            PreviousStatus = previousStatus;
        }
    }
}

using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SendingTenantDeactivationRequestEvent : SendingRequestBaseEvent
    {
        public SendingTenantDeactivationRequestEvent(Guid tenantId, Guid productId, Guid subscriptionId, TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
            : base(tenantId, productId, subscriptionId, status, step, previousStatus, previousStep)
        {

        }
    }
}
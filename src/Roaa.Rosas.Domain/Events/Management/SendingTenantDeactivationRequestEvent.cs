using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SendingTenantDeactivationRequestEvent : SendingRequestBaseEvent
    {
        public SendingTenantDeactivationRequestEvent(Guid tenantId, Guid productId, Guid subscriptionId, ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
            : base(tenantId, productId, subscriptionId, expectedResourceStatus, status, step, previousStatus, previousStep)
        {

        }
    }
}
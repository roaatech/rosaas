using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SendingTenantActivationRequestEvent : SendingRequestBaseEvent
    {
        public SendingTenantActivationRequestEvent(Guid tenantId, Guid productId, Guid subscriptionId, ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
            : base(tenantId, productId, subscriptionId, expectedResourceStatus, status, step, previousStatus, previousStep)
        {

        }
    }
}

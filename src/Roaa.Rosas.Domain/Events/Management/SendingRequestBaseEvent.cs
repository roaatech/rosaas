using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SendingRequestBaseEvent : BaseInternalEvent
    {
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SubscriptionId { get; set; }
        public TenantStatus Status { get; set; }
        public TenantStep Step { get; set; }
        public TenantStatus PreviousStatus { get; set; }
        public TenantStep PreviousStep { get; set; }

        public SendingRequestBaseEvent(Guid tenantId, Guid productId, Guid subscriptionId, TenantStatus status, TenantStep step, TenantStatus previousStatus, TenantStep previousStep)
        {
            TenantId = tenantId;
            ProductId = productId;
            SubscriptionId = subscriptionId;
            Status = status;
            Step = step;
            PreviousStatus = previousStatus;
            PreviousStep = previousStep;
        }
    }
}

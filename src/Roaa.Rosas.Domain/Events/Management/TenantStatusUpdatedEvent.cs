using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TenantStatusUpdatedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }
        public Workflow Workflow { get; set; }
        public TenantStatus PreviousStatus { get; set; }
        public TenantStep PreviousStep { get; set; }
        public string Notes { get; set; } = string.Empty;


        public TenantStatusUpdatedEvent(Subscription subscription, Workflow workflow, TenantStatus previousStatus, TenantStep previousStep, string notes = "")
        {
            Subscription = subscription;
            Workflow = workflow;
            PreviousStatus = previousStatus;
            PreviousStep = previousStep;
            Notes = notes;
        }
    }
}

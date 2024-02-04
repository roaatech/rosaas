using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public record Workflow
    {
        public WorkflowAction Action { get; set; }
        public TenantStatus NextStatus { get; set; }
        public TenantStatus CurrentStatus { get; set; }
        public TenantStep CurrentStep { get; set; }
        public TenantStep NextStep { get; set; }
        public List<UserType> OwnerTypes { get; set; } = new();
        public ExpectedTenantResourceStatus? ExpectedResourceStatus { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public WorkflowTrack Track { get; set; } = WorkflowTrack.Normal;
        public ICollection<WorkflowEventEnum>? Events { get; set; }
    }

    public class WorkflowEvent
    {
        public string Type { get; set; } = string.Empty;
        public WorkflowEventEnum FriendlyId { get; set; }
    }

    public class OrderWorkflowEvent
    {
        public string Type { get; set; } = string.Empty;
        public OrderIntent OrderIntent { get; set; }
    }


    public class StepStatus
    {
        public TenantStatus Status { get; set; }
        public TenantStep? Step { get; set; }
    }
    public enum WorkflowEventEnum
    {
        SendingTenantCreationRequestEvent = 1,
        SendingTenantActivationRequestEvent = 2,
        SendingTenantDeactivationRequestEvent = 3,
        SendingTenantDeletionRequestEvent = 4,
        TenantActivatedEvent = 5,
    }
    public enum WorkflowTrack
    {
        Normal = 1,
    }
    public enum WorkflowAction
    {
        Ok = 1,
        Cancel = 2,
    }
}

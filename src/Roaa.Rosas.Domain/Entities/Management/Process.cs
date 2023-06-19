using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public record Process
    {
        public WorkflowAction Action { get; set; }
        public TenantStatus NextStatus { get; set; }
        public TenantStatus CurrentStatus { get; set; }
        public UserType OwnerType { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public WorkflowTrack Track { get; set; } = WorkflowTrack.Normal;

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

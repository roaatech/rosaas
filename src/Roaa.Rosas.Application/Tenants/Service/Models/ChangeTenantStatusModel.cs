using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Service.Models
{
    public record ChangeTenantStatusModel
    {
        public Guid TenantId { get; init; }
        public Guid? ProductId { get; init; }
        public Guid EditorBy { get; init; }
        public WorkflowAction Action { get; init; }
        public TenantStatus Status { get; init; }
        public UserType UserType { get; init; }
    }
}

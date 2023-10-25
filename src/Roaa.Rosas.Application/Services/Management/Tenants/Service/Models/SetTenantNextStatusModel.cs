using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record SetTenantNextStatusModel
    {
        public Guid TenantId { get; init; }
        public Guid? ProductId { get; init; }
        public Guid EditorBy { get; init; }
        public WorkflowAction Action { get; init; }
        public TenantStatus Status { get; init; }
        public TenantStep? Step { get; init; }
        public ExpectedTenantResourceStatus? ExpectedResourceStatus { get; set; }
        public UserType UserType { get; init; }
        public string Notes { get; init; } = string.Empty;
    }
}

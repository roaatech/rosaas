using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantProcessesByTenantId
{
    public record TenantProcessDto
    {
        public Guid TenantId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStatus PreviousStatus { get; set; }

        public Guid OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime Created { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}

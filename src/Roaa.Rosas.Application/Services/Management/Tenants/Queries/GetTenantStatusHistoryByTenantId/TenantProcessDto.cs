using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantStatusHistoryByTenantId
{
    public record TenantProcessDto
    {
        public Guid TenantId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public TenantStatus PreviousStatus { get; set; }

        public TenantStep PreviousStep { get; set; }

        public Guid OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime Created { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}

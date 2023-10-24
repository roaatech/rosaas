using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantProcessesByTenantId
{
    public record TenantProcessDto
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public TenantProcessType ProcessType { get; set; }

        public string Data { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public Guid? OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime ProcessDate { get; set; }

        public int UpdatesCount { get; set; }

    }
}

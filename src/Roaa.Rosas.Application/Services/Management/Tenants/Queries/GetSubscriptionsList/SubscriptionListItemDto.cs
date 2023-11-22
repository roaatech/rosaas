using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public record SubscriptionListItemDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public bool IsActive { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}

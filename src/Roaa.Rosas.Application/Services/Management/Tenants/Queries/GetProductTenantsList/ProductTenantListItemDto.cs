using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetProductTenantsList
{
    public record ProductTenantListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}

using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Queries.GetProductTenantsList
{
    public record ProductTenantListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public TenantStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}

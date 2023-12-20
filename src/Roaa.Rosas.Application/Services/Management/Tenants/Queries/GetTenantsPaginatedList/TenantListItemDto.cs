using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsPaginatedList
{
    public record TenantListItemDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
        public TenantStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }

    public record SubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}

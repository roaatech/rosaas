using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenantsPaginatedList
{
    public record TenantListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public IEnumerable<LookupItemDto<Guid>> Products { get; set; } = new List<LookupItemDto<Guid>>();
        public TenantStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}

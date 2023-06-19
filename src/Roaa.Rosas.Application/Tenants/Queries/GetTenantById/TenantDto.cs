using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantById
{
    public record TenantDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public IEnumerable<LookupItemDto<Guid>> Products { get; set; } = new List<LookupItemDto<Guid>>();
        public TenantStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();

    }
}

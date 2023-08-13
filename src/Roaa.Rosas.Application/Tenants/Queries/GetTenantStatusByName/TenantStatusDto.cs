using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Queries.GetTenantStatusByName
{
    public record TenantStatusDto
    {
        public bool IsActive { get; set; }
        public TenantStatus Status { get; set; }

    }
}

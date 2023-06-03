using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Tenant : BaseAuditableEntity
    {
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public TenantStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ProductTenant>? Products { get; set; }
    }
}

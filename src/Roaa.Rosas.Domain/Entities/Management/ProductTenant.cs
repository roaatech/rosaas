using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class ProductTenant : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public TenantStatus Status { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public DateTime Edited { get; set; }
        public Guid EditedByUserId { get; set; }
        public string Metadata { get; set; } = string.Empty;
        public virtual Tenant? Tenant { get; set; }
        public virtual Product? Product { get; set; }


    }



}

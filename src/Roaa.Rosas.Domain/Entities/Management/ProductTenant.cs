namespace Roaa.Rosas.Domain.Entities.Management
{
    public class ProductTenant : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public virtual Tenant? Tenant { get; set; }
        public virtual Product? Product { get; set; }
    }
}

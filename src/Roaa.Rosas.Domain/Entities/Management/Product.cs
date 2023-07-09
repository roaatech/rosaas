namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Product : BaseAuditableEntity
    {
        public Guid ClientId { get; set; }
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string CreationEndpoint { get; set; } = string.Empty;
        public string ActivationEndpoint { get; set; } = string.Empty;
        public string DeactivationEndpoint { get; set; } = string.Empty;
        public string DeletionEndpoint { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<ProductTenant>? Tenants { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
    }

}

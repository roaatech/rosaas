namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Product : BaseAuditableEntity
    {
        public Guid ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DefaultHealthCheckUrl { get; set; } = string.Empty;
        public string HealthStatusInformerUrl { get; set; } = string.Empty;
        public string CreationUrl { get; set; } = string.Empty;
        public string ActivationUrl { get; set; } = string.Empty;
        public string DeactivationUrl { get; set; } = string.Empty;
        public string DeletionUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<ProductTenant>? Tenants { get; set; }
        public virtual ICollection<Feature>? Features { get; set; }
        public virtual ICollection<Plan>? Plans { get; set; }
    }

}

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SpecificationValue : BaseAuditableEntity
    {
        public Guid TenantId { get; set; }

        public Guid SpecificationId { get; set; }

        public Guid SubscriptionId { get; set; }

        public string Data { get; set; } = string.Empty;

        public virtual Tenant? Tenant { get; set; }

        public virtual Specification? Specification { get; set; }

        public virtual Subscription? Subscription { get; set; }
    }
}
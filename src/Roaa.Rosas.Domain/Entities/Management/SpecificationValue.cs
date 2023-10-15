namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SpecificationValue : BaseAuditableEntity
    {
        public Guid TenantId { get; set; }

        public Guid SpecificationId { get; set; }

        public Guid SubscriptionId { get; set; }

        public string? Value { get; set; }

        public virtual Tenant? Tenant { get; set; }

        public virtual Specification? Specification { get; set; }

        public virtual Subscription? Subscription { get; set; }
    }
}
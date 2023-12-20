namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Tenant : BaseAuditableEntity
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<SpecificationValue>? SpecificationsValues { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }

}

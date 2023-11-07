namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Plan : BaseAuditableEntity
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ICollection<PlanFeature>? Features { get; set; }
        public virtual ICollection<PlanPrice>? Prices { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<SubscriptionAutoRenewal>? SubscriptionAutoRenewals { get; set; }
    }
}
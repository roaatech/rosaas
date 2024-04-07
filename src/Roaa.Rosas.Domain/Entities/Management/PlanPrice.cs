namespace Roaa.Rosas.Domain.Entities.Management
{
    public class PlanPrice : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public decimal Price { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public virtual Plan? Plan { get; set; }
        public bool IsLockedBySystem { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<SubscriptionRenewal>? SubscriptionRenewals { get; set; }
    }


    public enum PlanCycle
    {
        [Obsolete("This property is obsolete. Use other instead.", false)]
        Week = 2,
        [Obsolete("This property is obsolete. Use other instead.", false)]
        OneDay = 5,
        [Obsolete("This property is obsolete. Use other instead.", false)]
        ThreeDays = 6,


        Month = 3,
        Year = 4,
        Custom = 10,
        Unlimited = 11,
    }
}
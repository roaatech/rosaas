namespace Roaa.Rosas.Domain.Entities.Management
{
    public class PlanPrice : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<SubscriptionAutoRenewal>? SubscriptionAutoRenewals { get; set; }
        public virtual ICollection<SubscriptionPlanChanging>? SubscriptionPlanChanges { get; set; }
    }


    public enum PlanCycle
    {
        Week = 2,
        Month = 3,
        Year = 4,
        OneDay = 5,
        ThreeDays = 6
    }
}
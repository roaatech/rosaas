namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionAutoRenewal : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public int? UpcomingAutoRenewalsCount { get; set; }
        public bool IsPaid { get; set; }
        public string? Comment { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual PlanPrice? PlanPrice { get; set; }
        public virtual Subscription? Subscription { get; set; }

    }

}

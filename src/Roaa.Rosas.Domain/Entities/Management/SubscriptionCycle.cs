namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionCycle : BaseAuditableEntity
    {
        public string PlanDisplayName { get; set; } = string.Empty;
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Subscription? Subscription { get; set; }
    }

}

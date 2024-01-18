namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionCycle : BaseAuditableEntity
    {
        public SubscriptionCycleType Type { get; set; }
        public string PlanDisplayName { get; set; } = string.Empty;
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual Subscription? Subscription { get; set; }
        public virtual ICollection<SubscriptionFeatureCycle>? SubscriptionFeaturesCycles { get; set; }
    }
    public enum SubscriptionCycleType
    {
        Normal = 1,
        Trial = 2,
    }
}

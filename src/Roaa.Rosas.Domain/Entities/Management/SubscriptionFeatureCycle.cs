namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionFeatureCycle : BaseAuditableEntity
    {
        public Guid SubscriptionFeatureId { get; set; }
        public Guid SubscriptionCycleId { get; set; }
        public string FeatureDisplayName { get; set; } = string.Empty;
        public Guid PlanFeatureId { get; set; }
        public Guid FeatureId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public PlanCycle Cycle { get; set; }
        public FeatureUnit? Unit { get; set; }
        public int? TotalUsage { get; set; }
        public int? RemainingUsage { get; set; }
        public int? Limit { get; set; }
        public virtual SubscriptionFeature? SubscriptionFeature { get; set; }
    }

}

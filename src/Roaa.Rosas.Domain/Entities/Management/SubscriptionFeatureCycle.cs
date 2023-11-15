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
        public FeatureType FeatureType { get; set; }
        public FeatureReset FeatureReset { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public FeatureUnit? FeatureUnit { get; set; }
        public int? TotalUsage { get; set; }
        public int? RemainingUsage { get; set; }
        public int? Limit { get; set; }
    }

}

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionFeature : BaseAuditableEntity
    {
        public Guid SubscriptionFeatureCycleId { get; set; }
        public Guid PlanFeatureId { get; set; }
        public Guid FeatureId { get; set; }
        public Guid SubscriptionId { get; set; }
        public int? RemainingUsage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual PlanFeature? PlanFeature { get; set; }
        public virtual Feature? Feature { get; set; }
        public virtual Subscription? Subscription { get; set; }
        public virtual ICollection<SubscriptionFeatureCycle>? SubscriptionFeatureCycles { get; set; }
    }

}

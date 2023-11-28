using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class PlanFeature : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public int? Limit { get; set; }
        public FeatureReset FeatureReset { get; set; }
        public FeatureUnit? FeatureUnit { get; set; }
        public LocalizedString UnitDisplayName { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public virtual Feature Feature { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<SubscriptionFeature>? SubscriptionFeatures { get; set; }
    }
}
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Models
{
    public class PlanFeatureInfoModel
    {
        public Guid GeneratedSubscriptionFeatureId { get; set; }
        public Guid GeneratedSubscriptionFeatureCycleId { get; set; }
        public Guid PlanFeatureId { get; set; }
        public Guid PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public int? Limit { get; set; }
        public FeatureType FeatureType { get; set; }
        public FeatureUnit? FeatureUnit { get; set; }
        public FeatureReset FeatureReset { get; set; }
        public string FeatureDisplayName { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
    }
}

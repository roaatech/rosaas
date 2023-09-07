namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Feature : BaseAuditableEntity
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public bool IsSubscribed { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<PlanFeature> Plans { get; set; }
        public virtual ICollection<SubscriptionFeature>? SubscriptionFeatures { get; set; }
    }

    public enum FeatureType
    {
        Number = 1,
        Boolean = 2,
    }
    public enum FeatureUnit
    {
        K = 1,
        MB = 2,
        GB = 3,
    }
    public enum FeatureReset
    {
        Never = 1,
        Weekly = 2,
        Monthly = 3,
        Annual = 4,
    }
}
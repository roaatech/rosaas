﻿namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Feature : BaseAuditableEntity
    {
        public Guid ProductId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public FeatureReset FeatureReset { get; set; }
        public bool IsSubscribed { get; set; }
        public int DisplayOrder { get; set; }
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
        item = 1,
        MB = 2,
        GB = 3,
        K = 4,
    }
    public enum FeatureReset
    {
        NonResettable = 1,
        Weekly = 2,
        Monthly = 3,
        Annual = 4,
        Daily = 5
    }
}
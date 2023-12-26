namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Product : BaseAuditableEntity
    {
        public Guid ClientId { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DefaultHealthCheckUrl { get; set; } = string.Empty;
        public string? HealthStatusInformerUrl { get; set; } = string.Empty;
        public string? CreationUrl { get; set; } = string.Empty;
        public string? ActivationUrl { get; set; } = string.Empty;
        public string? DeactivationUrl { get; set; } = string.Empty;
        public string? DeletionUrl { get; set; } = string.Empty;
        public string? SubscriptionResetUrl { get; set; }
        public string? SubscriptionUpgradeUrl { get; set; }
        public string? SubscriptionDowngradeUrl { get; set; }
        public string? ApiKey { get; set; } = string.Empty;
        public ProductTrialType TrialType { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? TrialPlanId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<Feature>? Features { get; set; }
        public virtual ICollection<Plan>? Plans { get; set; }
        public virtual ICollection<Specification>? Specifications { get; set; }
    }

    public enum ProductTrialType
    {
        None = 0,
        NoTrial = 1,
        ProductHasTrialPlan,
        EachPlanHasOptinalTrialPeriod
    }
}

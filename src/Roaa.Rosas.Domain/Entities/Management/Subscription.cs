using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Subscription : BaseAuditableEntity
    {
        public Guid SubscriptionCycleId { get; set; }
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public TenantStatus Status { get; set; }
        public TenantStep Step { get; set; }
        public bool IsActive { get; set; }
        public ExpectedTenantResourceStatus ExpectedResourceStatus { get; set; } = ExpectedTenantResourceStatus.None;
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public string Metadata { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime? LastResetDate { get; set; }
        public DateTime? ResetOperationDate { get; set; }
        public DateTime? LastLimitsResetDate { get; set; }
        public SubscriptionResetStatus? SubscriptionResetStatus { get; set; }
        public int? CustomPeriodInDays { get; set; } = null;
        public SubscriptionMode SubscriptionMode { get; set; }
        public virtual TrialSubscription? Trial { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual PlanPrice? PlanPrice { get; set; }
        public virtual Tenant? Tenant { get; set; }
        public virtual Product? Product { get; set; }
        public virtual TenantHealthStatus? HealthCheckStatus { get; set; }
        public virtual SubscriptionRenewal? SubscriptionRenewal { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public virtual ICollection<SubscriptionFeature>? SubscriptionFeatures { get; set; }
        public virtual ICollection<SubscriptionCycle>? SubscriptionCycles { get; set; }
        public virtual ICollection<SpecificationValue>? SpecificationsValues { get; set; }

    }

    public class TrialSubscription : BaseEntity
    {
        public Guid SubscriptionId { get; set; }
        public Guid TrialPlanId { get; set; }
        public Guid TrialPlanPriceId { get; set; }
        public Guid SelectedPlanId { get; set; }
        public Guid SelectedPlanPriceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TrialPeriodInDays { get; set; }
        public SubscriptionTrialStatus Status { get; set; }
        public virtual Subscription? Subscription { get; set; }
    }

    public enum SubscriptionTrialStatus
    {
        None = 0,
        PendingPayment = 1,
        Processing = 2,
        FailedPayment = 3,
        Failure = 4,
        Done = 5,
    }


    public enum SubscriptionResetStatus
    {
        Pending = 1,
        InProgress = 2,
        Done = 3,
        Failure = 4,
    }


    public enum SubscriptionMode
    {
        Standard = 1,
        Trial = 2,
        [Obsolete("This Option is obsolete.", false)]
        PendingToNormal = 3,
    }
}

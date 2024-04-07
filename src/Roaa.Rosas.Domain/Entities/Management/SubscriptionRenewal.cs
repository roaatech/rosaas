using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionRenewal : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public SubscriptionRenewalTypeEnum Type { get; set; }
        public decimal Price { get; set; }
        public string PlanDisplayName { get; set; } = string.Empty;
        public DateTime SubscriptionRenewalDate { get; set; }
        public SubscriptionRenewalStatus Status { get; set; }
        public int RenewalsCount { get; set; }
        public bool IsContinuousRenewal { get; set; }
        public bool IsForced { get; set; }
        public string? Comment { get; set; }
        public UserType CreatedByUserType { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual PlanPrice? PlanPrice { get; set; }
        public virtual Subscription? Subscription { get; set; }
    }
    public enum SubscriptionRenewalTypeEnum
    {
        Upgrade = 1,
        Downgrade = 2,
        AutoRenewal = 3,

    }

    public enum SubscriptionRenewalStatus
    {
        None = 0,
        PendingPayment = 1,
        preparing = 2,
        PendingExternalSystem = 3,
        Processing = 4,
        FailedPayment = 5,
        Failure = 6,
        FailedExternalSystem = 7,
    }

}

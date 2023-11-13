﻿namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionPlanChanging : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public PlanChangingType Type { get; set; }
        public decimal Price { get; set; }
        public string PlanDisplayName { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string? Comment { get; set; }
        public virtual Plan? Plan { get; set; }
        public virtual PlanPrice? PlanPrice { get; set; }
        public virtual Subscription? Subscription { get; set; }
    }

    public enum PlanChangingType
    {
        Upgrade = 1,
        Downgrade = 2,
    }

}

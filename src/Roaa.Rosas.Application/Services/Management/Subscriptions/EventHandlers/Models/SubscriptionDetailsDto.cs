using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Subscriptions.Models
{
    public record SubscriptionDetailsDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid CurrentSubscriptionCycleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastResetDate { get; set; }
        public DateTime? LastLimitsResetDate { get; set; }
        public SubscriptionResetStatus? SubscriptionResetStatus { get; set; }


        [Obsolete("This property is obsolete", false)]
        public int? SubscriptionPlanChangeStatus { get; set; }
        public bool HasSubscriptionFeaturesLimitsResettable { get; set; }
        public bool IsActive { get; set; }
        public bool IsResettableAllowed { get; set; }
        public bool AutoRenewalIsEnabled { get; set; }
        public bool UpgradingIsEnabled { get; set; }
        public bool DowngradingIsEnabled { get; set; }

        [Obsolete("This property is obsolete", false)]
        public bool IsPlanChangeAllowed { get; set; }

        [Obsolete("This property is obsolete", false)]
        public bool IsSubscriptionResetUrlExists { get; set; }

        [Obsolete("This property is obsolete", false)]
        public bool IsSubscriptionUpgradeUrlExists { get; set; }

        [Obsolete("This property is obsolete", false)]
        public bool IsSubscriptionDowngradeUrlExists { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public PlanPriceDto PlanPrice { get; set; } = new();

        [Obsolete("This property is obsolete", false)]
        public SubscriptionAutoRenewalDto? AutoRenewal { get; set; }

        [Obsolete("This property is obsolete", false)]
        public SubscriptionPlanChangingDto? SubscriptionPlanChange { get; set; }
        public SubscriptionRenewalDto? SubscriptionRenewal { get; set; }
        public IEnumerable<SubscriptionCycleDto> SubscriptionCycles { get; set; } = new List<SubscriptionCycleDto>();
        public SubscriptionRenewalAction? SubscriptionRenewalAction { get; set; }

        public class SubscriptionAutoRenewalDto
        {
            public PlanCycle Cycle { get; set; }
            public decimal Price { get; set; }
            public string? Comment { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime EditedDate { get; set; }
        }
        public class SubscriptionPlanChangingDto
        {
            public string PlanDisplayName { get; set; } = string.Empty;
            public SubscriptionRenewalTypeEnum Type { get; set; }
            public PlanCycle Cycle { get; set; }
            public decimal Price { get; set; }
            public string? Comment { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime EditedDate { get; set; }
        }
        public class SubscriptionRenewalDto
        {
            public string PlanDisplayName { get; set; } = string.Empty;
            public SubscriptionRenewalStatus Status { get; set; }
            public SubscriptionRenewalTypeEnum Type { get; set; }
            public PlanCycle Cycle { get; set; }
            public decimal Price { get; set; }
            public int RenewalsCount { get; set; }
            public bool IsContinuousRenewal { get; set; }
            public DateTime SubscriptionRenewalDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime EditedDate { get; set; }
            public string? Comment { get; set; }
        }
    }

    public class SubscriptionCycleDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SubscriptionCycleType CycleType { get; set; }
    }


    public record PlanPriceDto
    {
        public Guid Id { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
    }

}

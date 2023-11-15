using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionDetails
{
    public record SubscriptionDetailsDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid CurrentSubscriptionCycleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? LastResetDate { get; set; }
        public DateTime? LastLimitsResetDate { get; set; }
        public SubscriptionResetStatus? SubscriptionResetStatus { get; set; }
        public SubscriptionPlanChangeStatus? SubscriptionPlanChangeStatus { get; set; }
        public bool HasSubscriptionFeaturesLimitsResettable { get; set; }
        public bool IsResettableAllowed { get; set; }
        public bool IsPlanChangeAllowed { get; set; }
        public bool IsSubscriptionResetUrlExists { get; set; }
        public bool IsSubscriptionUpgradeUrlExists { get; set; }
        public bool IsSubscriptionDowngradeUrlExists { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public PlanPriceDto PlanPrice { get; set; } = new();
        public SubscriptionAutoRenewalDto? AutoRenewal { get; set; }
        public SubscriptionPlanChangingDto? SubscriptionPlanChange { get; set; }
        public IEnumerable<SubscriptionFeatureDto> SubscriptionFeatures { get; set; } = new List<SubscriptionFeatureDto>();
        public IEnumerable<SubscriptionCycleDto> SubscriptionCycles { get; set; } = new List<SubscriptionCycleDto>();
    }

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
        public PlanChangingType Type { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }

    public class SubscriptionCycleDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public record PlanPriceDto
    {
        public Guid Id { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
    }

    public class SubscriptionFeatureDto
    {
        public Guid Id { get; set; }
        public Guid CurrentSubscriptionFeatureCycleId { get; set; }
        public FeatureDto Feature { get; set; } = new();
        public int? RemainingUsage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
    }
}

using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

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
        public bool HasSubscriptionFeaturesLimitsResettable { get; set; }
        public bool IsResettableAllowed { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public PlanPriceDto PlanPrice { get; set; } = new();
        public SubscriptionAutoRenewalDto? AutoRenewal { get; set; }
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
    public class SubscriptionCycleDto
    {
        public Guid Id { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
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
        public IEnumerable<SubscriptionFeatureCycleDto> SubscriptionFeaturesCycles { get; set; } = new List<SubscriptionFeatureCycleDto>();
    }
    public class SubscriptionFeatureCycleDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionCycleId { get; set; }
        public CustomLookupItemDto<Guid> Feature { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public FeatureUnit? Unit { get; set; }
        public int? TotalUsage { get; set; }
        public int? RemainingUsage { get; set; }
        public int? Limit { get; set; }
    }
    public class FeatureDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
    }
}

using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionsList
{
    public record SubscriptionDetailsDto
    {
        public Guid SubscriptionId { get; set; }
        public LookupItemDto<Guid> Plan { get; set; } = new();
        public PlanPriceDto PlanPrice { get; set; } = new();
        public IEnumerable<SubscriptionFeatureDto> SubscriptionFeatures { get; set; } = new List<SubscriptionFeatureDto>();
    }

    public record PlanPriceDto
    {
        public Guid Id { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
    }

    public class SubscriptionFeatureDto
    {
        public FeatureDto Feature { get; set; } = new();
        public int? RemainingUsage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class FeatureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
    }
}

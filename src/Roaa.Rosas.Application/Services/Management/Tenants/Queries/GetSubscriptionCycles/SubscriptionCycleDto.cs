using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionCycles
{
    public class SubscriptionCycleDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SubscriptionCycleType CycleType { get; set; }
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
}

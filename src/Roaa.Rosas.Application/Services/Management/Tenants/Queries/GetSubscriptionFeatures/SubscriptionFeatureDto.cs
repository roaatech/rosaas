using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetSubscriptionFeatures
{
    public class SubscriptionFeatureDto
    {
        public Guid Id { get; set; }
        public Guid CurrentSubscriptionFeatureCycleId { get; set; }
        public LookupItemDto<Guid> Feature { get; set; } = new();
        public int? RemainingUsage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
    }

}

using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record CreatePlanFeatureModel
    {
        public Guid PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public int? Limit { get; set; }
        public FeatureReset? Reset { get; set; }
        public FeatureUnit? Unit { get; set; }
        public LocalizedString UnitDisplayName { get; set; } = new();
        public string Description { get; set; } = string.Empty;

    }
}

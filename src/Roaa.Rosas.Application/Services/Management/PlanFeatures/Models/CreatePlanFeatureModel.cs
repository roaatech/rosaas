namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record CreatePlanFeatureModel
    {
        public Guid PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public int? Limit { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}

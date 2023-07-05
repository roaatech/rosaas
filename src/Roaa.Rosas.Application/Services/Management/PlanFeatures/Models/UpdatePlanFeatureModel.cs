namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record UpdatePlanFeatureModel
    {
        public int? Limit { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}

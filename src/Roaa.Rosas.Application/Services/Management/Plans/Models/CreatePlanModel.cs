namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record CreatePlanModel
    {
        public Guid ProductId { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? AlternativePlanId { get; set; }
        public Guid? AlternativePlanPriceId { get; set; }
    }
}

namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record UpdatePlanModel
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
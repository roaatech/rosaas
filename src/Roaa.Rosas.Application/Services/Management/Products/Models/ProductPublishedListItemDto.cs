using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductPublishedListItemDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public ProductTrialType TrialType { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? TrialPlanId { get; set; }
        public Guid? TrialPlanPriceId { get; set; }
    }
}

using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record PlanPricePublishedListItemDto
    {
        public Guid Id { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlanListItemDto Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLockedBySystem { get; set; }
    }
}

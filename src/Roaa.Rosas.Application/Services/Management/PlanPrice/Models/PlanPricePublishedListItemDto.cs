using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record PlanPricePublishedListItemDto
    {
        public Guid Id { get; set; }
        public Guid PlanId { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

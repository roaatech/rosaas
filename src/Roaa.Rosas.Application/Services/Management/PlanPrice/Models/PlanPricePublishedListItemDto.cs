using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record PlanPricePublishedListItemDto
    {
        public Guid Id { get; set; }
        public PlanModel Plan { get; set; } = new PlanModel();
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class PlanModel
    {
        public PlanModel(Guid id)
        {
            Id = id;
        }
        public PlanModel()
        {
        }
        public Guid Id { get; set; }
    }
}

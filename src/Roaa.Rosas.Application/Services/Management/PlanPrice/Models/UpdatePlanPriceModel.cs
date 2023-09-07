using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record UpdatePlanPriceModel
    {
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

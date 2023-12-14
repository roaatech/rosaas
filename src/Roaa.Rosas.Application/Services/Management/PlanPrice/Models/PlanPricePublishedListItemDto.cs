using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record PlanPricePublishedListItemDto
    {
        public Guid Id { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CustomLookupItemDto<Guid> Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
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

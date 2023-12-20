using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record ExternalSystemPlanListItemDto
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLockedBySystem { get; set; }
        public TenancyType TenancyType { get; set; }
        public string TenancyTypeName { get; set; } = string.Empty;
        public List<ExternalSystemPlanPriceListItemDto> Prices { get; set; } = new();
    }

    public record ExternalSystemPlanPriceListItemDto
    {
        public PlanCycle Cycle { get; set; }
        public string CycleName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLockedBySystem { get; set; }
    }
}

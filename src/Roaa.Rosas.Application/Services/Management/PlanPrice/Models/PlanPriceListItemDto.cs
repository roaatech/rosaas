using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.PlanPrices.Models
{
    public record PlanPriceListItemDto
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


    public record PlanListItemDto
    {
        public PlanListItemDto()
        {
        }


        public PlanListItemDto(Guid id, string systemName, string displayName, TenancyType tenancyType, bool isLockedBySystem)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
            TenancyType = tenancyType;
            IsLockedBySystem = isLockedBySystem;
        }

        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public TenancyType TenancyType { get; set; }
        public bool IsLockedBySystem { get; set; }
    }

    public record ProductListItemDto
    {
        public ProductListItemDto()
        {
        }


        public ProductListItemDto(Guid id, string systemName, string displayName)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
        }

        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}

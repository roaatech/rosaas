using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record PlanFeatureListItemDto
    {
        public Guid Id { get; set; }
        public int? Limit { get; set; }
        public FeatureReset Reset { get; set; }
        public FeatureUnit? Unit { get; set; }
        public LocalizedString UnitDisplayName { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public FeatureItemDto Feature { get; set; } = new();
        public PlanItemDto Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
    public record FeatureItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public bool IsSubscribed { get; set; }
        public FeatureReset Reset { get; set; }
        public int DisplayOrder { get; set; }
    }
    public record PlanItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
    }
}

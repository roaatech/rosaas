using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record PlanFeatureListItemDto
    {
        public Guid Id { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
        public string Description { get; set; } = string.Empty;
        public FeatureItemDto Feature { get; set; } = new();
        public LookupItemDto<Guid> Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
    public record FeatureItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
    }
}

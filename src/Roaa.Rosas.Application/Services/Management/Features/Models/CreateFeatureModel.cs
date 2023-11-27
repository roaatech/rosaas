using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Features.Models
{
    public record CreateFeatureModel
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public int DisplayOrder { get; set; }
    }
}

using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Models
{
    public class FeatureDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureType Type { get; set; }
        public FeatureReset Reset { get; set; }
        public int? Limit { get; set; }
        public FeatureUnit? Unit { get; set; }
    }
}

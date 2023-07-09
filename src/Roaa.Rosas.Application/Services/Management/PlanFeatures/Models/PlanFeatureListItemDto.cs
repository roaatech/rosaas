using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.PlanFeatures.Models
{
    public record PlanFeatureListItemDto
    {
        public Guid Id { get; set; }
        public int? Limit { get; set; }
        public string Description { get; set; } = string.Empty;
        public LookupItemDto<Guid> Feature { get; set; } = new();
        public LookupItemDto<Guid> Plan { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
    }
}

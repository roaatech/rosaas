using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record PlanListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LookupItemDto<Guid> Product { get; set; } = new();
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
    }
}

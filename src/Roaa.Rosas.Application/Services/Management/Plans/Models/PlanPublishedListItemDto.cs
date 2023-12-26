using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record PlanPublishedListItemDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LookupItemDto<Guid> Product { get; set; } = new();
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLockedBySystem { get; set; }
        public TenancyType TenancyType { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? AlternativePlanID { get; set; }
    }
}

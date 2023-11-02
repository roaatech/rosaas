using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Common.Models;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Queries.GetTenentByNameAndProductId
{
    public record ProductTenantDto
    {
        public string Title { get; set; } = string.Empty;
        public string UniqueName { get; set; } = string.Empty;
        public string HealthCheckUrl { get; set; } = string.Empty;
        public bool HealthCheckUrlIsOverridden { get; set; }
        public TenantStatus Status { get; set; }
        public TenantStep Step { get; set; }
        public bool IsPaid { get; set; }
        public object Metadata { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public LookupItemDto<Guid> Plan { get; set; } = new();
        public IEnumerable<SpecificationListItemDto> Specifications { get; set; } = new List<SpecificationListItemDto>();
    }
    public record SpecificationListItemDto
    {
        public string Name { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public LocalizedString? ValidationFailureDescription { get; set; }

        public string? Value { get; set; }
    }
}

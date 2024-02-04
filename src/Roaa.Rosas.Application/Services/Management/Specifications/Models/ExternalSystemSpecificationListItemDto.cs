using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Application.Services.Management.Specifications.Models
{
    public record ExternalSystemSpecificationListItemDto
    {
        public string SystemName { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public LocalizedString? ValidationFailureDescription { get; set; }

        public bool IsPublished { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

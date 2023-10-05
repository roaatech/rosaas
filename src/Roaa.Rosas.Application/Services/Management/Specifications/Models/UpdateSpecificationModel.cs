using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Specifications.Models
{
    public record UpdateSpecificationModel
    {
        public string Name { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public SpecificationInputType InputType { get; set; }

        public SpecificationDataType DataType { get; set; }

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public LocalizedString? ValidationFailureDescription { get; set; }
    }
}

﻿using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Specifications.Models
{
    public record SpecificationListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public SpecificationType FieldType { get; set; }

        public FieldDataType DataType { get; set; }

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public string? ValidationFailureDescription { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime EditedDate { get; set; }

        public bool IsPublished { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

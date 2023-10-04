using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Specification : BaseAuditableEntity
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NormalizedName { get; set; } = string.Empty;

        public LocalizedString DisplayName { get; set; } = new();

        public LocalizedString Description { get; set; } = new();

        public SpecificationType FieldType { get; set; }

        public FieldDataType DataType { get; set; }

        public bool IsRequired { get; set; }

        public bool IsUserEditable { get; set; }

        public string? RegularExpression { get; set; }

        public string? ValidationFailureDescription { get; set; }

        public bool IsPublished { get; set; }

        public bool IsSubscribed { get; set; }

        public virtual Product? Product { get; set; }

        public virtual ICollection<SpecificationValue>? Values { get; set; }
    }

    public enum SpecificationType
    {
        Checkbox = 1,
        DatePicker,
        Text,
        Number,
        OneChoiceDropdownList,
        MultichoiceDropdownList,
        File,
        Image,
        ExternalHyperlink,
        InternalHyperlink,
        AllValuesInLine,
    }


    public enum FieldDataType
    {
        Text = 1,
        Html,
        Number,
        NumberPercent,
        Date,
        Boolean,
    }
}
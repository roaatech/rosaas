namespace Roaa.Rosas.Domain.Entities.Management
{
    public class GenericAttribute : BaseEntity
    {
        public Guid EntityId { get; set; }

        public string KeyGroup { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

    }
}

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Client : BaseAuditableEntity
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
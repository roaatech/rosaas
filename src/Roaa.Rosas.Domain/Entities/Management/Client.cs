namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Client : BaseAuditableEntity
    {
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
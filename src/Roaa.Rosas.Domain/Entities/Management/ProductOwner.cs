namespace Roaa.Rosas.Domain.Entities.Management
{
    public class ProductOwner : BaseAuditableEntity
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? AdministratorUserId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Product>? Products { get; set; }


    }
}
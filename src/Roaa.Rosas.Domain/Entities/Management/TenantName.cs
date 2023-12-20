namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantName : BaseEntity
    {
        public Guid? TenantId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

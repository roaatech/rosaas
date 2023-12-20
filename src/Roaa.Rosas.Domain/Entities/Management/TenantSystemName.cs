namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantSystemName : BaseEntity
    {
        public Guid? TenantId { get; set; }
        public Guid ProductId { get; set; }
        public string SystemName { get; set; } = string.Empty;
    }
}

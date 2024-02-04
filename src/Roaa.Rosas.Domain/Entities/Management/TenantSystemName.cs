namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantSystemName : BaseEntity
    {
        public Guid? TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid TenantCreationRequestId { get; set; }
        public string TenantNormalizedSystemName { get; set; } = string.Empty;
    }
}

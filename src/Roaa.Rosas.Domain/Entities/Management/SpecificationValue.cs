namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SpecificationValue : BaseAuditableEntity
    {
        public Guid TenantId { get; set; }

        public Guid FieldId { get; set; }

        public string Data { get; set; } = string.Empty;

        public virtual Tenant? Tenant { get; set; }

        public virtual Specification? Field { get; set; }
    }
}
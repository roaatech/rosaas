namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantCreationRequest : BaseAuditableEntity
    {
        public string NormalizedSystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public virtual ICollection<TenantCreationRequestSpecification> Specifications { get; set; } = new List<TenantCreationRequestSpecification>();
    }

    public class TenantCreationRequestSpecification : BaseEntity
    {
        public Guid TenantCreationRequestId { get; set; }
        public Guid SpecificationId { get; set; }
        public Guid ProductId { get; set; }
        public string Value { get; set; } = string.Empty;
        public virtual TenantCreationRequest? TenantCreationRequest { get; set; }
    }

}

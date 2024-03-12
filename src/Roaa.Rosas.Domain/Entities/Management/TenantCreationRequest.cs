using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantCreationRequest : BaseAuditableEntity
    {
        public Guid OrderId { get; set; }
        public List<Guid> ProductIds { get; set; } = new();
        public string NormalizedSystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public UserType CreatedByUserType { get; set; }
        public bool AutoRenewalIsEnabled { get; set; }
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

using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantStatusHistory : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStatus PreviousStatus { get; set; }

        public TenantStep Step { get; set; }

        public TenantStep PreviousStep { get; set; }

        public ExpectedTenantResourceStatus ExpectedResourceStatus { get; set; }

        public Guid OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime Created { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; } = string.Empty;

        //  public virtual ProductTenant? Tenant { get; set; }
    }
}

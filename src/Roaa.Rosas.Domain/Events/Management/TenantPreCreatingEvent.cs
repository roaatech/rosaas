using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class TenantPreCreatingEvent : BaseInternalEvent
    {
        public ProductTenant ProductTenant { get; set; }
        public TenantStatus PreviousStatus { get; set; }

        public TenantPreCreatingEvent(ProductTenant productTenant, TenantStatus previousStatus)
        {
            ProductTenant = productTenant;
            PreviousStatus = previousStatus;
        }
    }
}

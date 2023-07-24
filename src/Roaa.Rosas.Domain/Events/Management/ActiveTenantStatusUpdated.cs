using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class ActiveTenantStatusUpdated : BaseInternalEvent
    {
        public ProductTenant ProductTenant { get; set; }

        public ActiveTenantStatusUpdated(ProductTenant productTenant)
        {
            ProductTenant = productTenant;
        }
    }
}

using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Tenants.Service.Models
{
    public record ChangeTenantStatusResult
    {
        public ChangeTenantStatusResult(ProductTenant tenant, Process process)
        {
            Process = process;
            ProductTenant = tenant;
        }

        public Process Process { get; set; }
        public ProductTenant ProductTenant { get; set; }
    }
}

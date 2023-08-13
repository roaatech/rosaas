using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record SetTenantNextStatusResult
    {
        public SetTenantNextStatusResult(ProductTenant tenant, Process process)
        {
            Process = process;
            ProductTenant = tenant;
        }

        public Process Process { get; set; }
        public ProductTenant ProductTenant { get; set; }
    }
}

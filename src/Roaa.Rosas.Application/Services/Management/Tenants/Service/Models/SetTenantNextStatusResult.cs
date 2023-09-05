using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record SetTenantNextStatusResult
    {
        public SetTenantNextStatusResult(Subscription tenant, Process process)
        {
            Process = process;
            ProductTenant = tenant;
        }

        public Process Process { get; set; }
        public Subscription ProductTenant { get; set; }
    }
}

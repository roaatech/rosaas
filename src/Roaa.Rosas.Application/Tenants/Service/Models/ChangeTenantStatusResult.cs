using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Tenants.Service.Models
{
    public record ChangeTenantStatusResult
    {
        public ChangeTenantStatusResult(Tenant tenant, Process process)
        {
            Process = process;
            Tenant = tenant;
        }

        public Process Process { get; set; }
        public Tenant Tenant { get; set; }
    }
}

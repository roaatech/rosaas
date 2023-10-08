using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record SetTenantNextStatusResult
    {
        public SetTenantNextStatusResult(Subscription tenant, Workflow process)
        {
            Process = process;
            ProductTenant = tenant;
        }

        public Workflow Process { get; set; }
        public Subscription ProductTenant { get; set; }
    }
}

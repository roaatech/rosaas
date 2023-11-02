using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service.Models
{
    public record SetTenantNextStatusResult
    {
        public SetTenantNextStatusResult(Subscription subscription, Workflow process)
        {
            Process = process;
            Subscription = subscription;
        }

        public Workflow Process { get; set; }
        public Subscription Subscription { get; set; }
    }
}

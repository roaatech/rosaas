using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants
{
    public class ActiveTenantStatusUpdated : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }

        public ActiveTenantStatusUpdated(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

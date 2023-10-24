using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class StatusOfActiveTenantIsUpdated : BaseInternalEvent
    {
        public Subscription Subscription { get; set; }

        public StatusOfActiveTenantIsUpdated(Subscription subscription)
        {
            Subscription = subscription;
        }
    }
}

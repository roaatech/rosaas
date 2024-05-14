using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionAutorenewalEnabledEvent : BaseInternalEvent
    {
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionAutorenewalEnabledEvent()
        {
        }

        public SubscriptionAutorenewalEnabledEvent(SubscriptionRenewal subscriptionRenewal)
        {
            SubscriptionRenewal = subscriptionRenewal;

        }
    }
}

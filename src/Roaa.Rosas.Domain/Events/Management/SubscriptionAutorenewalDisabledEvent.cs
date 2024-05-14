using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionAutorenewalDisabledEvent : BaseInternalEvent
    {
        public SubscriptionRenewal SubscriptionRenewal { get; set; } = new();

        public SubscriptionAutorenewalDisabledEvent()
        {
        }

        public SubscriptionAutorenewalDisabledEvent(SubscriptionRenewal subscriptionRenewal)
        {
            SubscriptionRenewal = subscriptionRenewal;

        }
    }
}

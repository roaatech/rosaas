using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    [SubscriptionRenewalType(SubscriptionRenewalTypeEnum.AutoRenewal)]
    public class SubscriptionAutoRenewalDisabledEvent : SubscriptionRenewalHasBeenDisabledBaseEvent
    {
        public SubscriptionAutoRenewalDisabledEvent(SubscriptionRenewal subscriptionRenewal)
            : base(subscriptionRenewal)
        {
        }

        public SubscriptionAutoRenewalDisabledEvent()
        {
        }
    }
}

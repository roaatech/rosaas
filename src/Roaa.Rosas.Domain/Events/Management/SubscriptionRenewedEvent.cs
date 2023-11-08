using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionRenewedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionAutoRenewal SubscriptionAutoRenewal { get; set; } = new();
        public string SystemComment { get; set; } = string.Empty;

        public SubscriptionRenewedEvent()
        {
        }

        public SubscriptionRenewedEvent(Subscription subscription,
                                        SubscriptionAutoRenewal subscriptionAutoRenewal,
                                        string systemComment)
        {
            Subscription = subscription;
            SubscriptionAutoRenewal = subscriptionAutoRenewal;
            SystemComment = systemComment;
        }
    }
}

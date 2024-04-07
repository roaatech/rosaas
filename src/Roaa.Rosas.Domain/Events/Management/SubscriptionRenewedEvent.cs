using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionRenewedEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public SubscriptionRenewal SubscriptionAutoRenewal { get; set; } = new();
        public string SystemComment { get; set; } = string.Empty;

        public SubscriptionRenewedEvent()
        {
        }

        public SubscriptionRenewedEvent(Subscription subscription,
                                        SubscriptionRenewal subscriptionAutoRenewal,
                                        string systemComment)
        {
            Subscription = subscription;
            SubscriptionAutoRenewal = subscriptionAutoRenewal;
            SystemComment = systemComment;
        }
    }
}

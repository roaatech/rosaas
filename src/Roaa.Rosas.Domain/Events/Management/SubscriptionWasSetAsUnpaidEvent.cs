using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionWasSetAsUnpaidEvent : BaseInternalEvent
    {
        public Subscription Subscription { get; set; } = new();
        public string systemComment { get; set; } = string.Empty;

        public SubscriptionWasSetAsUnpaidEvent()
        {
        }

        public SubscriptionWasSetAsUnpaidEvent(Subscription subscription, string systemComment)
        {
            Subscription = subscription;
            this.systemComment = systemComment;
        }
    }
}

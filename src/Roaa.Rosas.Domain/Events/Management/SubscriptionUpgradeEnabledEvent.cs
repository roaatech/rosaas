using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionUpgradeEnabledEvent : BaseInternalEvent
    {
        public Guid SubscriptionId { get; }
        public Guid NewPlanId { get; }
        public Guid NewPlanPriceId { get; }
        public SubscriptionUpgradeEnabledEvent()
        {
        }

        public SubscriptionUpgradeEnabledEvent(Guid subscriptionId, Guid newPlanId, Guid newPlanPriceId)
        {
            SubscriptionId = subscriptionId;
            NewPlanId = newPlanId;
            NewPlanPriceId = newPlanPriceId;
        }
    }
}

using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionForcedDowngradeEnabledEvent : BaseInternalEvent
    {
        public Guid SubscriptionId { get; }
        public Guid NewPlanId { get; }
        public Guid NewPlanPriceId { get; }
        public SubscriptionForcedDowngradeEnabledEvent()
        {
        }

        public SubscriptionForcedDowngradeEnabledEvent(Guid subscriptionId, Guid newPlanId, Guid newPlanPriceId)
        {
            SubscriptionId = subscriptionId;
            NewPlanId = newPlanId;
            NewPlanPriceId = newPlanPriceId;
        }
    }
}

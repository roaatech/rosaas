using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionDowngradeEnabledEvent : BaseInternalEvent
    {
        public Guid SubscriptionId { get; }
        public Guid NewPlanId { get; }
        public Guid NewPlanPriceId { get; }
        public SubscriptionDowngradeEnabledEvent()
        {
        }

        public SubscriptionDowngradeEnabledEvent(Guid subscriptionId, Guid newPlanId, Guid newPlanPriceId)
        {
            SubscriptionId = subscriptionId;
            NewPlanId = newPlanId;
            NewPlanPriceId = newPlanPriceId;
        }
    } }

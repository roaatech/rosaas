using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class TrialSubscriptionUpgradedToStandardEvent : BaseInternalEvent
    {
        public TrialSubscriptionUpgradedToStandardEvent(TrialSubscription trial, Guid productId)
        {
            Trial = trial;
            ProductId = productId;
        }

        public TrialSubscription Trial { get; set; } = new();
        public Guid ProductId { get; set; }
    }
}

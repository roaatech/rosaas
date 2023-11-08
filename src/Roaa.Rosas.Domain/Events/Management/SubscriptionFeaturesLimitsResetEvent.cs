using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class SubscriptionFeaturesLimitsResetEvent : BaseInternalEvent
    {
        public List<SubscriptionFeatureItemModel> SubscriptionFeatures { get; set; } = new();
        public Subscription Subscription { get; set; }
        public string? Comment { get; set; }
        public string? SystemComment { get; set; }

        public SubscriptionFeaturesLimitsResetEvent(List<SubscriptionFeatureItemModel> subscriptionFeatures, Subscription subscription, string? comment, string? systemComment)
        {
            SubscriptionFeatures = subscriptionFeatures;
            Subscription = subscription;
            Comment = comment;
            SystemComment = systemComment;
        }
    }
}

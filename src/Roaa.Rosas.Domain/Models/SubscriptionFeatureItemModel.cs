namespace Roaa.Rosas.Domain.Models
{
    public class SubscriptionFeatureItemModel
    {
        public SubscriptionFeatureItemModel(Guid subscriptionFeatureId, string name)
        {
            SubscriptionFeatureId = subscriptionFeatureId;
            SystemName = name;
        }
        public SubscriptionFeatureItemModel()
        {
        }

        public Guid SubscriptionFeatureId { get; set; }
        public string SystemName { get; set; } = string.Empty;
    }
}

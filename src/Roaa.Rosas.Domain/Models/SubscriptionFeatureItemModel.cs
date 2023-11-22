namespace Roaa.Rosas.Domain.Models
{
    public class SubscriptionFeatureItemModel
    {
        public SubscriptionFeatureItemModel(Guid subscriptionFeatureId, string name)
        {
            SubscriptionFeatureId = subscriptionFeatureId;
            Name = name;
        }
        public SubscriptionFeatureItemModel()
        {
        }

        public Guid SubscriptionFeatureId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionFeatureUsage : BaseAuditableEntity
    {
        public Guid PlanFeatureId { get; set; }
        public Guid FeatureId { get; set; }
        public Guid SubscriptionId { get; set; }
        public int Usage { get; set; }
    }

}

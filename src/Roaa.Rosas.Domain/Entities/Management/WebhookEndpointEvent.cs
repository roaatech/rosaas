namespace Roaa.Rosas.Domain.Entities.Management
{
    public class WebhookEndpointEvent : BaseEntity
    {
        public Guid WebhookEndpointId { get; set; }
        public WebhookEvents Event { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual WebhookEndpoint? WebhookEndpoint { get; set; }

    }
    public enum WebhookEvents
    {
        MetadataUpdated = 1,
        TenantStatusChanged = 2,
        TenantAvailabilityChangedToHealthy = 3,
        TenantAvailabilityChangedToUnhealthy = 4,
        TrialSubscriptionUpgradedToStandard = 5,
        RenewalCanceled = 6,
        SubscriptionHasBeenRenewedAutomatically = 7,
        SubscriptionHasBeenUpgraded = 8,
        UpgradeEnabled = 9,
        DowngradeEnabled = 10,
        ForcedDowngradeEnabled = 11,
        SubscriptionHasBeenDowngraded = 12,
        SubscriptionSuspendedDueToUnpaid = 13,

    }

}

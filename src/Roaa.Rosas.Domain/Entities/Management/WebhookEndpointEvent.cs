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
        TenantRegisteredInRoSaasDb = 1,
        TenantStatusChanged = 2,
        TenantAvailabilityChangedToHealthy = 3,
        TenantAvailabilityChangedToUnhealthy = 4,
        TrialSubscriptionUpgradedToStandard = 5,
        SubscriptionRenewalDisabled = 6,
        SubscriptionHasBeenRenewedAutomatically = 7,
        SubscriptionHasBeenUpgraded = 8,
        UpgradeEnabled = 9,
        DowngradeEnabled = 10,
        ForcedDowngradeEnabled = 11,
        SubscriptionHasBeenDowngraded = 12,
        SubscriptionSuspendedDueToUnpaid = 13,
        AutoRenewalEnabled = 14,
        SubscriptionAutoRenewalDisabled = 15,
        SubscriptionUpgradingDisabled = 16,
        SubscriptionDowngradingDisabled = 17,
        ExternalSystemIsBeingProvisionedTenantResources = 18,
        ExternalSystemCreatedTenantResources = 19,


    }

}

using Roaa.Rosas.Common.Localization;

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
        [Localization(En = "Tenant.RegisteredInRosasDb", Ar = "")]
        TenantRegisteredInRoSaasDb = 1,

        [Localization(En = "Tenant.StatusChanged", Ar = "")]
        TenantStatusChanged = 2,

        [Localization(En = "Tenant.AvailabilityChangedToHealthy", Ar = "")]
        TenantAvailabilityChangedToHealthy = 3,

        [Localization(En = "Tenant.AvailabilityChangedToUnhealthy", Ar = "")]
        TenantAvailabilityChangedToUnhealthy = 4,

        [Localization(En = "Subscription.Trial.UpgradedToStandard", Ar = "")]
        TrialSubscriptionUpgradedToStandard = 5,

        [Localization(En = "Subscription.Renewal.Disabled", Ar = "")]
        SubscriptionRenewalDisabled = 6,

        [Localization(En = "Subscription.HasBeenRenewedAutomatically", Ar = "")]
        SubscriptionHasBeenRenewedAutomatically = 7,

        [Localization(En = "Subscription.HasBeenUpgraded", Ar = "")]
        SubscriptionHasBeenUpgraded = 8,

        [Localization(En = "Subscription.Upgrade.Enabled", Ar = "")]
        UpgradeEnabled = 9,

        [Localization(En = "Subscription.Downgrade.Enabled", Ar = "")]
        DowngradeEnabled = 10,

        [Localization(En = "Subscription.ForcedDowngrade.Enabled", Ar = "")]
        ForcedDowngradeEnabled = 11,

        [Localization(En = "Subscription.HasBeenDowngraded", Ar = "")]
        SubscriptionHasBeenDowngraded = 12,

        [Localization(En = "Subscription.SuspendedDueToUnpaid", Ar = "")]
        SubscriptionSuspendedDueToUnpaid = 13,

        [Localization(En = "Subscription.AutoRenewal.Enabled", Ar = "")]
        AutoRenewalEnabled = 14,

        [Localization(En = "Subscription.AutoRenewal.Disabled", Ar = "")]
        SubscriptionAutoRenewalDisabled = 15,

        [Localization(En = "Subscription.UpgradingDisabled", Ar = "")]
        SubscriptionUpgradingDisabled = 16,

        [Localization(En = "Subscription.DowngradingDisabled", Ar = "")]
        SubscriptionDowngradingDisabled = 17,

        [Localization(En = "ExternalSystem.IsBeingProvisionedTenantResources", Ar = "")]
        ExternalSystemIsBeingProvisionedTenantResources = 18,

        [Localization(En = "ExternalSystem.CreatedTenantResources", Ar = "")]
        ExternalSystemCreatedTenantResources = 19,

    }

}

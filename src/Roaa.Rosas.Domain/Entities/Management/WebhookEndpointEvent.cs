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
        // Metadata Events
        MetadataUpdated = 1,

        // Status Events
        StatusChanged = 2,
        StatusHealthy = 3,
        StatusUnhealthy = 4,

        // External System Events
        ExternalSystemSuccessfullyInformed = 5,
        ExternalSystemFailedToInform = 6,

        // Subscription Events
        WasSetAsUnpaidForNonRenewal = 7,
        SpecificationsUpdated = 8,
        ResetPrepared = 9,
        ResetAppliedDone = 10,
        ResetApplicationFailed = 11,
        FeatureLimitReset = 12,
        AutoRenewalEnabled = 13,
        RenewalCanceled = 14,
        Renewed = 15,
        RenewedFailed = 16,
        Created = 17,
        Updated = 18,
        UpgradeRequested = 19,
        UpgradePrepared = 20,
        UpgradeBeingApplied = 21,
        UpgradeApplicationFailed = 22,
        UpgradeAppliedDone = 23,
        UpgradeEnabled = 24,
        UpgradeDisabled = 25,
        DowngradeEnabled = 26,
        DowngradeDisabled = 27,
        ForcedDowngradeEnabled = 28,
        TrialStarted = 29,
        TrialEnded = 30
    }

}

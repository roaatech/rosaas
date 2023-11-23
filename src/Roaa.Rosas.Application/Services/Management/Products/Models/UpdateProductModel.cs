namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record UpdateProductModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string DefaultHealthCheckUrl { get; set; } = string.Empty;
        public string HealthStatusChangeUrl { get; set; } = string.Empty;
        public string CreationEndpoint { get; set; } = string.Empty;
        public string ActivationEndpoint { get; set; } = string.Empty;
        public string DeactivationEndpoint { get; set; } = string.Empty;
        public string DeletionEndpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string? SubscriptionResetUrl { get; set; }
        public string? SubscriptionUpgradeUrl { get; set; }
        public string? SubscriptionDowngradeUrl { get; set; }
    }
}

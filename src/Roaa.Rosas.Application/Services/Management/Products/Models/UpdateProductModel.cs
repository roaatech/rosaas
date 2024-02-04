namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record UpdateProductModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DefaultHealthCheckUrl { get; set; }
        public string? HealthStatusChangeUrl { get; set; }
        public string? CreationEndpoint { get; set; }
        public string? ActivationEndpoint { get; set; }
        public string? DeactivationEndpoint { get; set; }
        public string? DeletionEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? SubscriptionResetUrl { get; set; }
        public string? SubscriptionUpgradeUrl { get; set; }
        public string? SubscriptionDowngradeUrl { get; set; }
    }
}

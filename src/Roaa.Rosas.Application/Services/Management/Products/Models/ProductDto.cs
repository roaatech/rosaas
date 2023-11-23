using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public LookupItemDto<Guid> Client { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
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
        public int WarningsNum { get; set; }

    }
}

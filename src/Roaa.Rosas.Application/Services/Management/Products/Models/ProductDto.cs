using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string DefaultHealthCheckUrl { get; set; } = string.Empty;
        public string HealthStatusChangeUrl { get; set; } = string.Empty;
        public LookupItemDto<Guid> Client { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public string CreationEndpoint { get; set; } = string.Empty;
        public string ActivationEndpoint { get; set; } = string.Empty;
        public string DeactivationEndpoint { get; set; } = string.Empty;
        public string DeletionEndpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;

    }
}

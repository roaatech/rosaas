using Newtonsoft.Json;

namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record ActivateTenantModel
    {
        [JsonProperty(PropertyName = "tenantName")]
        public string TenantName { get; set; } = string.Empty;
    }
}

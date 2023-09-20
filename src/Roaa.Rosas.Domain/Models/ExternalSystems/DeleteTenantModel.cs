using Newtonsoft.Json;

namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record DeleteTenantModel
    {
        [JsonProperty(PropertyName = "tenantName")]
        public string TenantName { get; set; } = string.Empty;
    }
}

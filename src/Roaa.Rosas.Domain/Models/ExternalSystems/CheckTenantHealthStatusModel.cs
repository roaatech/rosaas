using Newtonsoft.Json;

namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CheckTenantHealthStatusModel
    {
        [JsonProperty(PropertyName = "tenantName")]
        public string TenantName { get; set; } = string.Empty;
    }
}

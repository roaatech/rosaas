namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CheckTenantHealthStatusModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}

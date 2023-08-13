namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record ActivateTenantModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}

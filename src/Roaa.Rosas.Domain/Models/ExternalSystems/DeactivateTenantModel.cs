namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record DeactivateTenantModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}

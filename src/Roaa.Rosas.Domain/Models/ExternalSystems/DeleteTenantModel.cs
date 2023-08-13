namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record DeleteTenantModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}

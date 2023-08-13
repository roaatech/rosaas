namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record InformTheTenantUnavailableModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}

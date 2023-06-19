namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CreateTenantModel
    {
        public Guid TenantId { get; set; }
        public string TenantUniqueName { get; set; } = string.Empty;
        public string TenantTitle { get; set; } = string.Empty;
    }
}

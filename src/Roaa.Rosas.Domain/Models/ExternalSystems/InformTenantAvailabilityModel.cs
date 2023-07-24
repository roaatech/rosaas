namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record InformTenantAvailabilityModel
    {
        public Guid TenantId { get; set; }
        public bool IsAvailable { get; set; }
    }
}

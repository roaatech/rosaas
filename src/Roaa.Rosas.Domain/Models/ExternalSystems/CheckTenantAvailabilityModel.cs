namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CheckTenantAvailabilityModel
    {
        public Guid TenantId { get; set; }
    }
}

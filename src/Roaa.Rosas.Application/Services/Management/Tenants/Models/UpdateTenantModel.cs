namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record UpdateTenantModel
    {
        public Guid Id { get; set; }
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

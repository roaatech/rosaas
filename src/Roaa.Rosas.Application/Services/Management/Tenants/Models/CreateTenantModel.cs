namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record CreateTenantModel
    {
        public Guid ProductId { get; set; }
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

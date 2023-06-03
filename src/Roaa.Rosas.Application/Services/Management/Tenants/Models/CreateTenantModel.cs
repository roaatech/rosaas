namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record CreateTenantModel
    {
        public List<Guid> ProductsIds { get; set; } = new();
        public string UniqueName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}

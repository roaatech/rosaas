namespace Roaa.Rosas.Application.Services.Management.Tenants.Models
{
    public record UpdateTenantStatusModel
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}

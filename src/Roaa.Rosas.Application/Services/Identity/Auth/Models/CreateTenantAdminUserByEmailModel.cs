namespace Roaa.Rosas.Application.Services.Identity.Auth.Models;

public record CreateTenantAdminUserByEmailModel : CreateUserByEmailModel
{
    public Guid TenantId { get; set; }
}
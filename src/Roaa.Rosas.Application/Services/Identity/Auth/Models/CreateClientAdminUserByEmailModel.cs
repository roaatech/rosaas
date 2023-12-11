namespace Roaa.Rosas.Application.Services.Identity.Auth.Models;

public record CreateClientAdminUserByEmailModel : CreateUserByEmailModel
{
    public Guid ClientId { get; set; }
}

namespace Roaa.Rosas.Application.Services.Identity.Auth.Models;

public record CreateProductAdminUserByEmailModel : CreateUserByEmailModel
{
    public Guid ProductId { get; set; }
}

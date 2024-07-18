namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password;

public record ValidateUserCodeModel
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password;

public record ForgotPasswordModel
{
    public string Email { get; set; } = string.Empty;
}
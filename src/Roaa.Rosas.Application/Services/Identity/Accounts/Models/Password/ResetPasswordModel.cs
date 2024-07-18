namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password;

public record ResetPasswordModel
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
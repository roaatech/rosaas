namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password
{
    public record ChangeMyPasswordModel
    {
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}

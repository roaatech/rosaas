namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models
{
    public record ConfirmEmailModel
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

}

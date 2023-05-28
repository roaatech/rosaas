namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models
{
    public record AccountResultModel<T>
    {
        public UserAccountItem? UserAccount { get; set; }
        public T? Details { get; set; }
    }
}

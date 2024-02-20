using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models
{
    public record UserCustomerDto
    {
        public UserAccountItem? UserAccount { get; set; }
        public CustomerModel? CustomerData { get; set; }
    }
}

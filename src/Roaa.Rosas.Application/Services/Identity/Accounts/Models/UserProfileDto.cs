using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Models
{
    public record UserProfileDto
    {
        public UserAccountItem? UserAccount { get; set; }
        public UserProfileModel? UserProfile { get; set; }
    }
}

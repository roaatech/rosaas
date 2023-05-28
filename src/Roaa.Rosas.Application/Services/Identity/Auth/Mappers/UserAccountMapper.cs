using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Application.Services.Identity.Auth.Mappers
{
    public static class UserAccountMapper
    {
        public static UserAccountDto ToUserAccountDto(this User user)
        {
            return new UserAccountDto
            {
                Email = user.Email,
                Locale = user.Locale,
                Id = user.Id,
                UserType = user.UserType,
                EmailConfirmed = user.EmailConfirmed,
            };
        }
    }
}

using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Application.Interfaces
{
    public interface IAuthTokenService
    {
        Task<Result<TokenModel>> GenerateAsync(Guid userId, string clientId, AuthenticationMethod authenticationMethod);
        Task<Result<TokenModel>> GenerateAsync(User user, string clientId, AuthenticationMethod authenticationMethod);
    }
}

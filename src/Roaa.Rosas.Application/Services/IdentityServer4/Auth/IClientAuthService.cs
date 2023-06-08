using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Auth
{
    public interface IClientAuthService
    {
        Task<Result<AuthResultModel>> AuthClientAsync(AuthClientModel model, CancellationToken cancellationToken = default);
    }
}

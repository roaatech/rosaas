using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Auth
{
    public interface IAuthService
    {
        Task<Result<AuthResultModel<AdminDto>>> SignInAdminByEmailAsync(SignInAdminByEmailModel model, CancellationToken cancellationToken = default);
    }
}

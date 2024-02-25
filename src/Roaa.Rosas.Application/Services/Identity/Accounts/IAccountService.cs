using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Identity.Accounts
{
    public interface IAccountService
    {
        Task<Result<AccountResultModel<dynamic>>> GetCurrentUserAccountAsync(CancellationToken cancellationToken = default);

        Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Result> ChangePasswordAsync(ChangeMyPasswordModel model, CancellationToken cancellationToken = default);

        Task<Result> UpdateUserProfileAsync(Guid userId, UserProfileModel model, CancellationToken cancellationToken = default);
    }
}

using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts
{
    public interface IAccountService
    {
        Task<Result<AccountResultModel<dynamic>>> GetCurrentUserAccountAsync(CancellationToken cancellationToken = default);
    }
}

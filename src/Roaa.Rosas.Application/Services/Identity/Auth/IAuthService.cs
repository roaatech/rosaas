using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Application.Services.Identity.Auth
{
    public interface IAuthService
    {
        Task<Result<Guid>> CreateTenantAdminUserByEmailAsync(CreateTenantAdminUserByEmailModel model, CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateProductAdminUserByEmailAsync(CreateProductAdminUserByEmailModel model, CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateClientAdminUserByEmailAsync(CreateClientAdminUserByEmailModel model, CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateExternalSystemUserByUsernameAsync(string username,
                                                                    Guid productId,
                                                                    Guid userId,
                                                                    bool isLocked,
                                                                    CancellationToken cancellationToken = default);

        Task<Result<AuthResultModel<AdminDto>>> SignUpUserByEmailAsync(SignUpUserByEmailModel model, UserType userType, CancellationToken cancellationToken = default);

        Task<Result<AuthResultModel<AdminDto>>> SignInAdminByEmailAsync(SignInUserByEmailModel model, CancellationToken cancellationToken = default);

        Task<Result<CheckResultModel>> EnsureEmailIsUniqueAsync(string email, CancellationToken cancellationToken);
    }
}

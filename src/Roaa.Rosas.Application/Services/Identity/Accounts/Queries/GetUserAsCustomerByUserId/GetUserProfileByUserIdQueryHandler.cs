using MediatR;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Queries.GetUserProfileByUserId
{
    public class GetUserProfileByUserIdQueryHandler : IRequestHandler<GetUserProfileByUserIdQuery, Result<UserProfileDto>>
    {
        #region Props   
        private readonly IAccountService _accountService;
        #endregion


        #region Corts
        public GetUserProfileByUserIdQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #endregion


        #region Handler   
        public async Task<Result<UserProfileDto>> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await _accountService.GetUserProfileAsync(request.Id, cancellationToken);
        }
        #endregion
    }
}

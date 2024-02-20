using MediatR;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts.Queries.GetUserAsCustomerByUserId
{
    public class GetUserAsCustomerByUserIdQueryHandler : IRequestHandler<GetUserAsCustomerByUserIdQuery, Result<UserCustomerDto>>
    {
        #region Props   
        private readonly IAccountService _accountService;
        #endregion


        #region Corts
        public GetUserAsCustomerByUserIdQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #endregion


        #region Handler   
        public async Task<Result<UserCustomerDto>> Handle(GetUserAsCustomerByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await _accountService.GetUserAsCustomerAsync(request.Id, cancellationToken);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Identity.Accounts
{
    public class AccountService : IAccountService
    {
        #region Props  
        private readonly IRosasIdentityDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        #endregion


        #region Corts
        public AccountService(
            IRosasIdentityDbContext dbContext,
            IIdentityContextService identityContextService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
        }
        #endregion

        #region Services   


        #region Admin Services  
        public async Task<Result<AccountResultModel<dynamic>>> GetCurrentUserAccountAsync(CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.AsNoTracking()
                                                   .Where(x => x.Id == _identityContextService.UserId)
                                                   .SingleOrDefaultAsync(cancellationToken);
            var result = new AccountResultModel<dynamic>
            {
                UserAccount = new UserAccountItem
                {
                    Email = user.Email,
                    Locale = user.Locale,
                    Id = user.Id,
                    UserType = user.UserType,
                    EmailConfirmed = user.EmailConfirmed,
                },
            };

            return Result<AccountResultModel<dynamic>>.Successful(result);
        }
        #endregion

        #endregion
    }

}

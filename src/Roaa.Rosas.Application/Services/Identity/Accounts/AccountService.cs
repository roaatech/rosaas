using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Identity.Accounts
{
    public class AccountService : IAccountService
    {
        #region Props  
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IGenericAttributeService _genericAttributeService;
        #endregion


        #region Corts
        public AccountService(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService,
            IGenericAttributeService genericAttributeService)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _genericAttributeService = genericAttributeService;
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



        public async Task<Result<UserCustomerDto>> GetUserAsCustomerAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.AsNoTracking()
                                                 .Where(x => x.Id == _identityContextService.UserId)
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Result<UserCustomerDto>.Successful(default(UserCustomerDto));
            }

            var customerData = await _genericAttributeService.GetAttributeAsync<User, CustomerModel>(userId, Consts.GenericAttributeKey.CustomerData, null, cancellationToken);

            var result = new UserCustomerDto
            {
                CustomerData = customerData,
                UserAccount = new UserAccountItem
                {
                    Id = user.Id,
                    Email = user.Email,
                    Locale = user.Locale,
                    UserType = user.UserType,
                    EmailConfirmed = user.EmailConfirmed,
                },
            };

            return Result<UserCustomerDto>.Successful(result);
        }
        #endregion

        #endregion
    }

}

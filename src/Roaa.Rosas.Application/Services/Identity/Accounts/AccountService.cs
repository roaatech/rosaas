using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Application.Services.Identity.Accounts.Validators;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Identity.Accounts
{
    public class AccountService : IAccountService
    {
        #region Props  
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly UserManager<User> _userManager;
        private readonly IPublisher _publisher;
        #endregion


        #region Corts
        public AccountService(
            IRosasDbContext dbContext,
            IIdentityContextService identityContextService,
            IGenericAttributeService genericAttributeService,
            UserManager<User> userManager,
            IPublisher publisher)
        {
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _genericAttributeService = genericAttributeService;
            _userManager = userManager;
            _publisher = publisher;
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
                    Id = user.Id,
                    Email = user.Email,
                    Locale = user.Locale,
                    UserType = user.UserType,
                    EmailConfirmed = user.EmailConfirmed,
                },
            };

            return Result<AccountResultModel<dynamic>>.Successful(result);
        }



        public async Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.AsNoTracking()
                                                 .Where(x => x.Id == _identityContextService.UserId)
                                                 .SingleOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return Result<UserProfileDto>.Successful(default(UserProfileDto));
            }

            var profile = await _genericAttributeService.GetAttributeAsync<User, UserProfileModel>(userId, Consts.GenericAttributeKey.UserProfile, null, cancellationToken);

            var result = new UserProfileDto
            {
                UserProfile = profile,
                UserAccount = new UserAccountItem
                {
                    Id = user.Id,
                    Email = user.Email,
                    Locale = user.Locale,
                    UserType = user.UserType,
                    EmailConfirmed = user.EmailConfirmed,
                },
            };

            return Result<UserProfileDto>.Successful(result);
        }

        public async Task<Result> UpdateUserProfileAsync(Guid userId, UserProfileModel model, CancellationToken cancellationToken = default)
        {
            var userProfile = new UserProfileModel
            {
                FullName = model.FullName,
                MobileNumber = model.MobileNumber
            };

            await _genericAttributeService.SaveAttributeAsync<User, UserProfileModel>(userId,
                                                                                    Consts.GenericAttributeKey.UserProfile,
                                                                                    userProfile,
                                                                                    cancellationToken);
            await _publisher.Publish(new UserProfileModelEvent(userId, userProfile), cancellationToken);
            return Result.Successful();
        }

        public async Task<Result> ChangePasswordAsync(ChangeMyPasswordModel model, CancellationToken cancellationToken = default)
        {
            #region Validation

            var uoValidation = new ChangeMyPasswordModelValidator(_identityContextService).Validate(model);
            if (!uoValidation.IsValid)
            {
                return Result.New().WithErrors(uoValidation.Errors);
            }

            var user = await _userManager.FindByIdAsync(_identityContextService.UserId.ToString());

            if (!user.IsActive)
            {
                return Result.Fail(ErrorMessage.UserAccountNotActive, _identityContextService.Locale);
            }

            #endregion

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return result.FailResult(_identityContextService.Locale);
            }

            return Result.Successful();
        }

        #endregion

        #endregion
    }

}

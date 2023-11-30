using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Auth.Mappers;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Application.Services.Identity.Auth.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Application.Services.Identity.Auth
{
    public class AuthService : IAuthService
    {
        #region Props
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        private readonly IAuthTokenService _tokenService;
        private readonly ValidationBuilder _validationBuilder;
        #endregion


        #region Corts
        public AuthService(
        UserManager<User> userManager,
            ILogger<AuthService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            IIdentityContextService identityContextService,
            IAuthTokenService tokenService)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _identityContextService = identityContextService;
            _tokenService = tokenService;
            _validationBuilder = new ValidationBuilder(identityContextService.Locale);
        }
        #endregion

        #region Services   

        #region SignIn (Admin - Web) 

        public async Task<Result<AuthResultModel<AdminDto>>> SignInAdminByEmailAsync(SignInAdminByEmailModel model, CancellationToken cancellationToken = default)
        {
            #region Validation 
            User user = null;
            _validationBuilder.AddCommand(() => new SignInAdminByEmailValidator(_identityContextService).Validate(model));
            _validationBuilder.AddCommand(async () => user = await _userManager.FindByEmailAsync(model.Email), ErrorMessage.InvalidLogin);
            //  _validationBuilder.AddCommand(() => user.UserType == UserType.SuperAdmin, ErrorMessage.InvalidLogin);
            _validationBuilder.AddCommand(() => user.IsActive, ErrorMessage.AccountDeactivated);
            _validationBuilder.AddCommand(async () => await _userManager.CheckPasswordAsync(user, model.Password), ErrorMessage.InvalidLogin);
            var validationResult = await _validationBuilder.ValidateAsync();
            #endregion

            return await SignInUserAsync<AdminDto>(user, AuthenticationMethod.Email, cancellationToken);
        }
        #endregion

        private async Task<Result<AuthResultModel<T>>> SignInUserAsync<T>(User user, AuthenticationMethod authenticationMethod, CancellationToken cancellationToken)
        {
            #region Validation   
            var validationResult = await _validationBuilder.ValidateAsync();
            if (!validationResult.Success)
            {
                return Result<AuthResultModel<T>>.Fail(validationResult.Messages);
            }
            #endregion

            if (!user.EmailConfirmed)
            {
                // await GenerateEmailConfirmationTokenAsync(user);

                return Result<AuthResultModel<T>>.Fail(ErrorMessage.UserMustConfirmTheirEmailAccount, _identityContextService.Locale);
            }

            user.LastLoginDate = DateTime.UtcNow;
            _dbContext.Entry(user).State = EntityState.Modified;

            var tokenResult = await _tokenService.GenerateAsync(user, _identityContextService.ClientId, authenticationMethod);

            if (!tokenResult.Success)
            {
                return Result<AuthResultModel<T>>.Fail(tokenResult.Messages);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);


            return Result<AuthResultModel<T>>.Successful(new AuthResultModel<T>
            {
                UserAccount = user.ToUserAccountDto(),
                Token = tokenResult.Data,
            });
        }





        #endregion





    }


}

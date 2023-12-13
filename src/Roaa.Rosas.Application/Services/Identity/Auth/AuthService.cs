using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Identity.Auth.Mappers;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Application.Services.Identity.Auth.Validators;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Events.Management;
using System.Data;

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
        private readonly IPublisher _publisher;
        private User _user = new();
        #endregion


        #region Corts
        public AuthService(
        UserManager<User> userManager,
            ILogger<AuthService> logger,
            IRosasDbContext dbContext,
            IWebHostEnvironment environment,
            IIdentityContextService identityContextService,
            IAuthTokenService tokenService,
            IPublisher publisher)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _environment = environment;
            _identityContextService = identityContextService;
            _tokenService = tokenService;
            _publisher = publisher;
            _validationBuilder = new ValidationBuilder(identityContextService.Locale);
        }
        #endregion

        #region Services   

        #region SignIn (Admin - Web) 

        public async Task<Result<AuthResultModel<AdminDto>>> SignInAdminByEmailAsync(SignInUserByEmailModel model, CancellationToken cancellationToken = default)
        {
            #region Validation  
            var allowedTypes = new UserType[] { UserType.SuperAdmin, UserType.ClientAdmin, UserType.ProductAdmin, UserType.TenantAdmin };
            User user = null;
            _validationBuilder.AddCommand(() => new SignInAdminByEmailValidator(_identityContextService).Validate(model));
            _validationBuilder.AddCommand(async () => user = await _userManager.FindByEmailAsync(model.Email), ErrorMessage.InvalidLogin);
            _validationBuilder.AddCommand(() => allowedTypes.Contains(user.UserType), ErrorMessage.InvalidLogin);
            _validationBuilder.AddCommand(() => user.IsActive, ErrorMessage.AccountDeactivated);
            _validationBuilder.AddCommand(async () => await _userManager.CheckPasswordAsync(user, model.Password), ErrorMessage.InvalidLogin);
            var validationResult = await _validationBuilder.ValidateAsync();
            #endregion

            return await SignInUserAsync<AdminDto>(user, AuthenticationMethod.Email, cancellationToken);
        }
        #endregion


        #region SignUp 

        public async Task<Result<AuthResultModel<AdminDto>>> SignUpUserByEmailAsync(SignUpUserByEmailModel model, UserType userType, CancellationToken cancellationToken = default)
        {
            #region Validation 
            _validationBuilder.AddCommand(() => new SignupUserByEmailValidator(_identityContextService).Validate(model));

            _validationBuilder.AddCommand(async () => await EnsureEmailIsUniqueAsync(model.Email, cancellationToken));

            var validationResult = await _validationBuilder.ValidateAsync();
            if (!validationResult.Success)
            {
                return Result<AuthResultModel<AdminDto>>.Fail(validationResult.Messages);
            }
            #endregion

            BuildUserEntity(model.Email, userType);

            using (var scope = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted))
            {
                var identityResult = await _userManager.CreateAsync(_user, model.Password);

                if (!identityResult.Succeeded)
                {
                    scope.Rollback();
                    return identityResult.FailResult<AuthResultModel<AdminDto>>(_identityContextService.Locale);
                }

                return await SignInUserAsync<AdminDto>(_user, AuthenticationMethod.Email, cancellationToken);
            }
        }


        public async Task<Result<Guid>> CreateTenantAdminUserByEmailAsync(CreateTenantAdminUserByEmailModel model, CancellationToken cancellationToken = default)
        {
            _validationBuilder.AddCommand(() => new CreateTenantAdminUserByEmailValidator(_identityContextService).Validate(model));

            var result = await CreateUserByEmailAsync(model, UserType.TenantAdmin, cancellationToken);

            if (result.Success)
                await _publisher.Publish(new UserCreatedAsTenantAdminEvent(_user, model.TenantId));

            return result;
        }

        public async Task<Result<Guid>> CreateProductAdminUserByEmailAsync(CreateProductAdminUserByEmailModel model, CancellationToken cancellationToken = default)
        {
            _validationBuilder.AddCommand(() => new CreateProductAdminUserByEmailValidator(_identityContextService).Validate(model));

            var result = await CreateUserByEmailAsync(model, UserType.ProductAdmin, cancellationToken);

            if (result.Success)
                await _publisher.Publish(new UserCreatedAsProductAdminEvent(_user, model.ProductId));

            return result;
        }

        public async Task<Result<Guid>> CreateClientAdminUserByEmailAsync(CreateClientAdminUserByEmailModel model, CancellationToken cancellationToken = default)
        {
            _validationBuilder.AddCommand(() => new CreateClientAdminUserByEmailValidator(_identityContextService).Validate(model));

            var result = await CreateUserByEmailAsync(model, UserType.ClientAdmin, cancellationToken);

            if (result.Success)
                await _publisher.Publish(new UserCreatedAsClientAdminEvent(_user, model.ClientId));

            return result;
        }

        public async Task<Result<Guid>> CreateUserByEmailAsync<TCreateUserByEmailModel>(
                                                                TCreateUserByEmailModel model,
                                                                UserType userType,
                                                                CancellationToken cancellationToken = default)
                                                                where TCreateUserByEmailModel : CreateUserByEmailModel
        {
            #region Validation  

            _validationBuilder.AddCommand(async () => await EnsureEmailIsUniqueAsync(model.Email, cancellationToken = default));

            var validationResult = await _validationBuilder.ValidateAsync();

            if (!validationResult.Success)
            {
                return Result<Guid>.Fail(validationResult.Messages);
            }
            #endregion

            BuildUserEntity(model.Email, userType);

            using (var scope = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted))
            {
                var identityResult = await _userManager.CreateAsync(_user, model.Password);

                if (!identityResult.Succeeded)
                {
                    scope.Rollback();
                    return identityResult.FailResult<Guid>(_identityContextService.Locale);
                }

                return Result<Guid>.Successful(_user.Id);
            }
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

                // return Result<AuthResultModel<T>>.Fail(ErrorMessage.UserMustConfirmTheirEmailAccount, _identityContextService.Locale);
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


        public async Task<Result<bool>> EnsureEmailIsUniqueAsync(string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result<bool>.Fail(CommonErrorKeys.InvalidParameters, _identityContextService.Locale, "email");
            }

            var any = await _dbContext.Users
                                      .AsNoTracking()
                                      .Where(x => email.ToLower().Equals(x.Email.ToLower()))
                                      .AnyAsync(cancellationToken);

            if (any)
            {
                return Result<bool>.Fail(ErrorMessage.AccountAlreadyExist, _identityContextService.Locale);
            }

            return Result<bool>.Successful(true);
        }


        private void BuildUserEntity(string email, UserType type)
        {
            _user.Id = Guid.NewGuid();
            _user.UserName = email;
            _user.Email = email;
            _user.CreationDate = DateTime.Now;
            _user.ModificationDate = DateTime.Now;
            _user.IsActive = true;
            _user.Locale = "en";
            _user.Status = UserStatus.Ready;
            _user.UserType = type;
        }



    }


}

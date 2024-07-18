using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models.Password;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.API.Controllers
{

    [Authorize(Policy = AuthPolicy.Identity.Account, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class AccountController : BaseIdentityApiController
    {
        #region Props 
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        #endregion

        #region Corts
        public AccountController(ILogger<AccountController> logger,
                                IWebHostEnvironment environment,
                                IAccountService accountService,
                                IIdentityContextService identityContextService)
        {
            _logger = logger;
            _environment = environment;
            _accountService = accountService;
            _identityContextService = identityContextService;
        }
        #endregion


        #region Actions   

        [HttpGet()]
        public async Task<IActionResult> GetCurrentUserAccountAsync(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetCurrentUserAccountAsync(cancellationToken);

            return ItemResult(result);
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUserProfileAsync(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetUserProfileAsync(_identityContextService.UserId, cancellationToken);

            return ItemResult(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UserProfileModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.UpdateUserProfileAsync(_identityContextService.UserId, model, cancellationToken));
        }


        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.ConfirmEmailAsync(model, cancellationToken));
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangeMyPasswordModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.ChangePasswordAsync(model, cancellationToken));
        }


        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordModel model, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _accountService.ForgotPasswordAsync(model, cancellationToken));
        }


        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.ResetPasswordAsync(model, cancellationToken));
        }

        #endregion


    }
}

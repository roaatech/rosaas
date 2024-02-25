using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Accounts.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
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


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangeMyPasswordModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.ChangePasswordAsync(model, cancellationToken));
        }


        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UserProfileModel model, CancellationToken cancellationToken)
        {
            return EmptyResult(await _accountService.UpdateUserProfileAsync(_identityContextService.UserId, model, cancellationToken));
        }

        #endregion


    }
}

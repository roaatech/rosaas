using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class AccountController : BaseAdminIdentityApiController
    {
        #region Props 
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _environment;
        private const string AdminPanelClientId = IdentityServer4Config.AdminPanelClientId;
        #endregion

        #region Corts
        public AccountController(ILogger<AccountController> logger,
                                IWebHostEnvironment environment,
                                IAuthService authService,
                                IAccountService accountService)
        {
            _logger = logger;
            _environment = environment;
            _authService = authService;
            _accountService = accountService;
        }
        #endregion


        #region Actions   

        [AllowAnonymous]
        [HttpPost("Signin")]
        public async Task<IActionResult> SignInAsync(SignInAdminByEmailModel model, CancellationToken cancellationToken)
        {
            if (!CheckClientId(AdminPanelClientId))
            {
                return InvalidRequest();
            }

            var result = await _authService.SignInAdminByEmailAsync(model, cancellationToken);

            return ItemResult(result);
        }




        [HttpGet("GetCurrentUserAccount")]
        public async Task<IActionResult> GetCurrentUserAccountAsync(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetCurrentUserAccountAsync();

            return ItemResult(result);
        }
        #endregion


    }
}

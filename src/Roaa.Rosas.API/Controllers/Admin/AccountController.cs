using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Identity.Accounts;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class AccountController : BaseSuperAdminIdentityApiController
    {
        #region Props 
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _environment;
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

        [HttpGet()]
        public async Task<IActionResult> GetCurrentUserAccountAsync(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetCurrentUserAccountAsync(cancellationToken);

            return ItemResult(result);
        }
        #endregion


    }
}

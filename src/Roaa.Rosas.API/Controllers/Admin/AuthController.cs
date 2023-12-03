using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{

    public class AuthController : BaseIdentityApiController
    {
        #region Props 
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;
        private const string AdminPanelClientId = SystemConsts.Clients.AdminPanel;
        #endregion

        #region Corts
        public AuthController(ILogger<AuthController> logger,
                                IWebHostEnvironment environment,
                                IAuthService authService)
        {
            _logger = logger;
            _environment = environment;
            _authService = authService;
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

        #endregion


    }
}

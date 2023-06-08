using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth;
using Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.ExternalSystem
{

    public class AuthController : BaseExternalSystemIdentityApiController
    {
        #region Props 
        private readonly ILogger<AuthController> _logger;
        private readonly IClientAuthService _authService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts
        public AuthController(ILogger<AuthController> logger,
                                IWebHostEnvironment environment,
                                IClientAuthService authService)
        {
            _logger = logger;
            _environment = environment;
            _authService = authService;
        }
        #endregion


        #region Actions   


        [AllowAnonymous]
        [HttpPost()]
        public async Task<IActionResult> AuthClientAsync(AuthClientModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.AuthClientAsync(model, cancellationToken);

            return ItemResult(result);
        }

        #endregion


    }
}

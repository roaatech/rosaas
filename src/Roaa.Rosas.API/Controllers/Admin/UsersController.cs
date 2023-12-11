using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Identity.Auth;
using Roaa.Rosas.Application.Services.Identity.Auth.Models;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{


    public class UsersController : BaseIdentityApiController
    {
        #region Props 
        private readonly ILogger<UsersController> _logger;
        private readonly IAuthService _authService;
        #endregion

        #region Corts
        public UsersController(ILogger<UsersController> logger,
                                IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }
        #endregion


        #region Actions   


        [Authorize(Policy = AuthPolicy.Identity.TeneatAdminUser, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("TenantAdmin")]
        public async Task<IActionResult> CreateTenantAdminUserByEmailAsync(CreateTenantAdminUserByEmailModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.CreateTenantAdminUserByEmailAsync(model, cancellationToken);

            return ItemResult(result);
        }


        [Authorize(Policy = AuthPolicy.Identity.ProductAdminUser, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("ProductAdmin")]
        public async Task<IActionResult> CreateProductAdminUserByEmailAsync(CreateProductAdminUserByEmailModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.CreateProductAdminUserByEmailAsync(model, cancellationToken);

            return ItemResult(result);
        }


        [Authorize(Policy = AuthPolicy.Identity.ClientAdminUser, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("ClientAdmin")]
        public async Task<IActionResult> CreateClientAdminUserByEmailAsync(CreateClientAdminUserByEmailModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.CreateClientAdminUserByEmailAsync(model, cancellationToken);

            return ItemResult(result);
        }
        #endregion


    }
}

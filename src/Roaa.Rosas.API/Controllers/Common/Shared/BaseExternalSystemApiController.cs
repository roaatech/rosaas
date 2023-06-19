using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Framework.Controllers.Common
{

    [Authorize(Policy = AuthPolicy.ExternalSystem, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public abstract class BaseExternalSystemApiController : BaseRosasApiController
    {
    }
}

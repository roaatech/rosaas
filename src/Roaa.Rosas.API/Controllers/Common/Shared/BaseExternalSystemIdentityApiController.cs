using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Framework.Controllers.Common
{
    [Route(ExternalSystemIdentityApiRout)]
    [Authorize(Policy = AuthPolicy.ExternalSystem, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public abstract class BaseExternalSystemIdentityApiController : BaseRosasApiController
    {
    }
}

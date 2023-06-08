using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Framework.Controllers.Common
{
    [Route(SuperAdminIdentityApiRout)]
    [Authorize(Policy = AuthPolicy.SuperAdmin, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public abstract class BaseSuperAdminIdentityApiController : BaseRosasApiController
    {
        protected const UserType Usertype = UserType.SuperAdmin;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Framework.Controllers.Common
{
    [Route(SuperAdminMainApiRout)]
    [Authorize(Policy = AuthPolicy.SuperAdmin)]
    public abstract class BaseSuperAdminMainApiController : BaseRosasApiController
    {
        protected const UserType Usertype = UserType.SuperAdmin;
    }
}

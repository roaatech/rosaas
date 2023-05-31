using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Common.Controllers;

namespace Roaa.Rosas.Framework.Controllers.Common
{

    [Route(IdentityApiRout)]
    public abstract class BaseRosasApiController : BaseApiController
    {
        public const string IdentityApiRout = "api/identity/v1/[controller]";
        public const string SuperAdminIdentityApiRout = "api/identity/management/v1/[controller]";
        public const string SuperAdminMainApiRout = "api/main/management/v1/[controller]";
    }
}

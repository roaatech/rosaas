using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Roaa.Rosas.Framework.Controllers.Common
{

    [AllowAnonymous]
    [Route(PublicApiRoute)]
    public abstract class BaseRosasPublicApiController : BaseRosasApiController
    {
    }
}

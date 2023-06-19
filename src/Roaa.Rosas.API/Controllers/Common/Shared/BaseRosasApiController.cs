using Roaa.Rosas.Common.Controllers;

namespace Roaa.Rosas.Framework.Controllers.Common
{

    public abstract class BaseRosasApiController : BaseApiController
    {
        public const string SuperAdminIdentityApiRout = "api/identity/sadmin/v1/[controller]";
        public const string SuperAdminMainApiRout = "api/management/sadmin/v1/[controller]";


    }
}

using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Application.JWT;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Utilities;

namespace Roaa.Rosas.Application.Extensions
{
    public static class IdentityContextExtensions
    {
        public static Guid GetProductId(this IIdentityContextService identityContext)
        {
            var value = identityContext.GetClaim(SystemConsts.Clients.Claims.ClaimProductId);

            return new Guid(value);
        }
        public static UserType GetUserType(this IIdentityContextService identityContext)
        {
            var value = identityContext.GetClaim(SystemConsts.Clients.Claims.ClaimType);
            var ss = value.ToPascalCaseNamingStrategy();
            var type = CommonHelper.ParseEnum<UserType>(value.ToPascalCaseNamingStrategy());
            return type;
        }
        public static Guid GetActorId(this IIdentityContextService identityContext)
        {
            return UserTypeManager.FromKey(identityContext.GetUserType()).GetActorId(identityContext);
        }
    }
}

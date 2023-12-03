using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Utilities;

namespace Roaa.Rosas.Application.IdentityContextUtilities
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
            if (string.IsNullOrWhiteSpace(value))
            {
                return UserType.RosasSystem;
            }
            var type = Helpers.ParseEnum<UserType>(value.ToPascalCaseNamingStrategy());
            return type;
        }
        public static Guid GetActorId(this IIdentityContextService identityContext)
        {
            return UserTypeManager.FromKey(identityContext.GetUserType()).GetActorId(identityContext);
        }
    }
}

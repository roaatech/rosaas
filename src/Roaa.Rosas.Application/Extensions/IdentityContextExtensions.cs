using Roaa.Rosas.Application.IdentityServer4;
using Roaa.Rosas.Authorization.Utilities;

namespace Roaa.Rosas.Application.Extensions
{
    public static class IdentityContextExtensions
    {
        public static Guid GetProductId(this IIdentityContextService identityContext)
        {
            var value = identityContext.GetClaim(SystemConsts.Clients.Claims.ClaimProductId);

            return new Guid(value);
        }
    }
}

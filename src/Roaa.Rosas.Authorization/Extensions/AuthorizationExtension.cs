using System.Security.Claims;
using System.Security.Principal;

namespace Roaa.Rosas.Authorization.Extensions
{
    public static class AuthorizationExtension
    {
        public static string GetClaim(this IIdentity identity, string claim)
        {
            return ((ClaimsIdentity)identity).Claims.FirstOrDefault((c) => c.Type == claim)?.Value;
        }

        public static List<string> GetClaims(this IIdentity identity, string claim)
        {
            return ((ClaimsIdentity)identity).Claims.Where((c) => c.Type == claim).Select(x => x.Value).ToList();
        }

        public static string GetUserIdAsString(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.SingleOrDefault((c) => c.Type.Equals("sub")).Value;
        }

        public static Guid GetUserId(this IIdentity identity)
        {
            return new Guid(identity.GetUserIdAsString());
        }

        public static string GetClientId(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.SingleOrDefault((c) => c.Type.Equals("client_id")).Value;
        }
    }
}

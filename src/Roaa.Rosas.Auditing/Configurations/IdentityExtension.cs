using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Roaa.Rosas.Auditing.Configurations;

public static class IdentityExtension
{

    internal static string GetClaim(this HttpContext httpContext, string key)
    {
        return ((ClaimsIdentity)httpContext.User.Identity).Claims.SingleOrDefault((c) => c.Type.Equals(key))?.Value;

    }
    internal static bool CheckIsAuthenticated(this HttpContext httpContext)
    {
        return httpContext?.User?.Identity?.IsAuthenticated == true ? true : false;
    }
}
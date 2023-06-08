using Microsoft.AspNetCore.Http;
using Roaa.Rosas.Authorization.Extensions;
using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Authorization.Utilities
{
    public class HttpContextService : IIdentityContextService, IDisposable
    {
        public HttpContext? HttpContext { get; private set; }

        #region Ctors     
        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext;
        }
        #endregion


        #region Props  

        public Guid UserId
        {
            get
            {
                return GetUserId();
            }
        }

        public string ClientId
        {
            get
            {
                return GetClientId();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return CheckIsAuthenticated();
            }
        }

        public LanguageEnum Locale
        {
            get
            {
                return GetLocale();
            }
        }

        public string HostUrl
        {
            get
            {
                return GetHostUrl();
            }
        }


        #endregion

        public string GetClaim(string claimType)
        {
            var value = HttpContext.User.Identity.GetClaim(claimType);

            return value;
        }



        #region private Props                                   
        private string? userObjectId;
        private Guid userGId;
        private string? clientId;
        private bool? isAuthenticated;
        private LanguageEnum? locale;

        #endregion




        private Guid GetUserId()
        {
            if (userGId == default(Guid))
            {
                if (CheckIsAuthenticated())
                    userGId = HttpContext.User.Identity.GetUserId();
            }
            return userGId;
        }

        private string GetClientId()
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                if (CheckIsAuthenticated())
                {
                    clientId = HttpContext.User.Identity.GetClientId();
                }
                else
                {
                    clientId = HttpContext.Request.Headers.FirstOrDefault(h => "Client-Id".Equals(h.Key, StringComparison.OrdinalIgnoreCase))
                                                        .Value
                                                        .ToString();
                }
            }
            return clientId;
        }

        private bool CheckIsAuthenticated()
        {
            if (isAuthenticated is null)
                isAuthenticated = HttpContext?.User?.Identity?.IsAuthenticated == true ? true : false;
            return (bool)isAuthenticated;
        }

        private LanguageEnum GetLocale()
        {
            if (locale is null)
            {
                locale = HttpContext.Request.Headers
                                            .FirstOrDefault(h => "Accept-Language".Equals(h.Key, StringComparison.OrdinalIgnoreCase))
                                            .Value
                                            .ToString()
                                            .ToLanguageOrDefault();
            }
            return locale.Value;
        }

        private string GetHostUrl()
        {
            return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
        }

        public void Dispose()
        {
            HttpContext = null;
        }
    }
}

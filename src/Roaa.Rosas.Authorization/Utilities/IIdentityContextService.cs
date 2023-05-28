using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Authorization.Utilities
{
    public interface IIdentityContextService
    {
        Guid UserId { get; }

        string ClientId { get; }

        bool IsAuthenticated { get; }

        LanguageEnum Locale { get; }

        string HostUrl { get; }
    }
}

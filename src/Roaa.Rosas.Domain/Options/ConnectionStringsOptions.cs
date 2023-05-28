using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Domain.Models.Options
{
    public record ConnectionStringsOptions : BaseOptions
    {
        public const string Section = "ConnectionStrings";
        public string IdS4ConfigurationDb { get; set; } = string.Empty;
        public string IdS4PersistedGrantDb { get; set; } = string.Empty;
        public string IdentityDb { get; set; } = string.Empty;
    }
}

using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Domain.Models.Options
{
    public record RootOptions : BaseOptions
    {
        // public const string Section = "IdentityApi";
        public IdentityServerOptions IdentityServer { get; set; }
        public ConnectionStringsOptions ConnectionStrings { get; set; }
        public GeneralOptions General { get; set; }
    }
}

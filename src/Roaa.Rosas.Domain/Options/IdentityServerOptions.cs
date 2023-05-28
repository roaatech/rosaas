using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Domain.Models.Options
{
    public record IdentityServerOptions : BaseOptions
    {
        public const string Section = "IdentityServer";
        public string Url { get; set; } = string.Empty;
        public string ApiName { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; }
        public bool UseInMemoryDatabase { get; set; }
        public bool MigrateDatabase { get; set; }
        public bool SeedData { get; set; }
        public string Jwk { get; set; } = string.Empty;
    }
}

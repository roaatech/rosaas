using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Domain.Models.Options
{
    public record GeneralOptions : BaseOptions
    {
        public const string Section = "General";
        public bool MigrateDatabase { get; set; }
        public bool SeedData { get; set; }
        public bool UseSingleDatabase { get; set; }
        public bool CreateDummyData { get; set; }
    }
}

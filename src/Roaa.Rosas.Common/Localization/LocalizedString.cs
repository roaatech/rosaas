using Newtonsoft.Json;

namespace Roaa.Rosas.Common.Localization
{
    public class LocalizedString
    {
        [JsonProperty("en")]
        public string En { get; set; } = string.Empty;

        [JsonProperty("ar")]
        public string Ar { get; set; } = string.Empty;
    }
}

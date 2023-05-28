using Newtonsoft.Json;

namespace Roaa.Rosas.Common.Localization
{
    public interface ILocalizedString
    {
        [JsonProperty("en")]
        string En { get; set; }

        [JsonProperty("ar")]
        string Ar { get; set; }
    }
}

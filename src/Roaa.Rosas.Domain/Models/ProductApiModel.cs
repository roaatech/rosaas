namespace Roaa.Rosas.Domain.Models
{
    public record ProductApiModel
    {
        public ProductApiModel(string apiKey, string url)
        {
            ApiKey = apiKey;
            Url = url;
        }
        public string ApiKey { get; set; }
        public string? Url { get; set; }
    }
}

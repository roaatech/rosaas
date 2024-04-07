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
    public record ExternalSystemApiModel : ProductApiModel
    {
        public ExternalSystemApiModel(string apiKey, string url, bool applySubscriptionRenewalByExternalSystemAction)
            : base(apiKey, url)
        {
            ApplySubscriptionRenewalByExternalSystemAction = applySubscriptionRenewalByExternalSystemAction;
        }
        public bool ApplySubscriptionRenewalByExternalSystemAction { get; set; }
    }
}

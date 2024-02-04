using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Domain.Models.Options
{
    public record PaymentOptions : BaseOptions
    {
        public const string Section = "Payment";
        public string SuccessPageUrl { get; set; } = string.Empty;
        public string CancelPageUrl { get; set; } = string.Empty;
        public Stripe Stripe { get; set; } = new();


    }
    public record Stripe
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}

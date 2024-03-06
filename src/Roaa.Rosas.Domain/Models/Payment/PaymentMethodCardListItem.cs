using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Models.Payment
{
    public record PaymentMethodCardListItem
    {
        public string StripeCardId { get; set; } = string.Empty;

        public string ReferenceId { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }

        public string CardholderName { get; set; } = string.Empty;

        public string Last4Digits { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public PaymentPlatform PaymentPlatform { get; set; }
    }
}

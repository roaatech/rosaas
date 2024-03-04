namespace Roaa.Rosas.Domain.Models
{
    public record PaymentMethodCardDto
    {
        public string ReferenceId { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CardholderName { get; set; } = string.Empty;
        public string Last4Digits { get; set; } = string.Empty;
    }
}

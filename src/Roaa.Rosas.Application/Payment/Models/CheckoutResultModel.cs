using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Models
{
    public record CheckoutResultModel
    {
        public string? NavigationUrl { get; set; }
        public Guid? TenantId { get; set; }
    }


    public record PaymentMethodCheckoutResultModel
    {
        public string? PaymentLink { get; set; }
    }


    public record CompleteSessionResultModel
    {
        public string? NavigationUrl { get; set; }
        public Order Order { get; set; } = new();
    }


}




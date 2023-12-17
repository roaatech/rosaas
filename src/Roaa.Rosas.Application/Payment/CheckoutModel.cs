using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment
{
    public record CheckoutModel
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid OrderId { get; set; }
        public PaymentMethodType? PaymentMethod { get; set; }
    }





}




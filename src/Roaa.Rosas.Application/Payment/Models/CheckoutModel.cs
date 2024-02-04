using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Models
{
    public record CheckoutModel
    {
        public Guid OrderId { get; set; }
        public PaymentMethodType? PaymentMethod { get; set; }
    }





}




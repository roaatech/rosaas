using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethodType? PaymentMethodType { get; set; }
        public CurrencyCode UserCurrencyType { get; set; }
        public string UserCurrencyCode { get; set; } = string.Empty;
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool HasToPay { get; set; }
    }
}

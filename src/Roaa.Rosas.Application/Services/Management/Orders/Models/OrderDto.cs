using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Orders.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
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
        public bool IsMustChangePlan { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SubscriptionId { get; set; }
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public int? CustomPeriodInDays { get; set; } = null;
        public string DisplayName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public int TrialPeriodInDays { get; set; }

    }
}

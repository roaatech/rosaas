using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Order : BaseAuditableEntity
    {
        public int OrderNumber { get; }
        public UserType UserType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethodType? PaymentMethodType { get; set; }
        public CurrencyCode UserCurrencyType { get; set; }
        public string UserCurrencyCode { get; set; } = string.Empty;
        public decimal CurrencyRate { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }


        // public decimal OrderSubTotalDiscountInclTax { get; set; }
        // public decimal OrderSubTotalDiscountExclTax { get; set; }
        //   public decimal PaymentMethodAdditionalFeeInclTax { get; set; }
        //   public decimal PaymentMethodAdditionalFeeExclTax { get; set; }
        //  public string TaxRates { get; set; } = string.Empty;
        //   public decimal OrderTax { get; set; }
        //   public decimal OrderDiscount { get; set; }


        public decimal OrderTotal { get; set; }

        public string? AuthorizationTransactionId { get; set; }

        public string? AuthorizationTransactionCode { get; set; }

        public string? AuthorizationTransactionResult { get; set; }

        public string? Reference { get; set; }

        public DateTime? PaidDate { get; set; }

        public Guid TenantId { get; set; }

        public virtual Tenant? Tenant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }

    public enum PaymentMethodType
    {
        Manwal = 1,
        Stripe = 2,
    }

    public enum PaymentStatus
    {
        Pending = 100,

        Authorized = 200,

        Paid = 300,

        Refunded = 400,

        Voided = 500,

        PartiallyRefunded = 600,

    }

    public enum CurrencyCode
    {
        USD = 1,
    }
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Complete = 3,
        Cancelled = 4
    }
}

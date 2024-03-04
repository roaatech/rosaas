using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Order : BaseAuditableEntity
    {
        public Guid? TenantId { get; set; }
        public int OrderNumber { get; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethodType? PaymentMethodType { get; set; }
        public DateTime? PaymentProcessingExpirationDate { get; set; } = null;
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

        public string? ProcessedPaymentId { get; set; }

        public string? AltProcessedPaymentId { get; set; }

        public string? ProcessedPaymentResult { get; set; }

        public string? CapturedPaymentResult { get; set; }

        public string? AuthorizedPaymentResult { get; set; }

        public string? ProcessedPaymentReference { get; set; }

        public string? ProcessedPaymentReferenceType { get; set; }

        public DateTime? PaidDate { get; set; }

        public Guid? PayerUserId { get; set; }

        public UserType PayerUserType { get; set; }

        public UserType CreatedByUserType { get; set; }

        public OrderIntent OrderIntent { get; set; }

        public bool IsMustChangePlan { get; set; }

        public PaymentMethodCard? PaymentMethodCard { get; set; }

        public virtual Tenant? Tenant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public class PaymentMethodCard
    {
        public string ReferenceId { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CardholderName { get; set; } = string.Empty;
        public string Last4Digits { get; set; } = string.Empty;
    }
    public enum PaymentMethodType
    {
        Manwal = 1,
        Stripe = 2,
    }

    public enum PaymentStatus
    {
        None = 1,

        Paid = 200,

        Authorized = 300,

        Refunded = 400,

        Failed = 500,

        Voided = 600,

        PartiallyRefunded = 700,
    }

    public enum CurrencyCode
    {
        USD = 1,
    }
    public enum OrderStatus
    {
        Initial = 1,
        PendingToPay = 2,
        Complete = 3,
        Cancelled = 4
    }
}

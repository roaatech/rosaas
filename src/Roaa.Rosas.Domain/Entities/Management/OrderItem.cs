using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; }
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
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        //public decimal DiscountAmountInclTax { get; set; }
        //public decimal DiscountAmountExclTax { get; set; }

        public int TrialPeriodInDays { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Subscription? Subscription { get; set; }
        public List<OrderItemSpecification> Specifications { get; set; } = new();

    }

    public class OrderItemSpecification
    {
        public Guid PurchasedEntityId { get; set; }
        public EntityType PurchasedEntityType { get; set; }
        public string SystemName { get; set; } = string.Empty;
    }
}

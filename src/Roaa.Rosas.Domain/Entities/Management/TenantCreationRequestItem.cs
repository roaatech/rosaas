namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantCreationRequestItem : BaseEntity
    {
        public Guid TenantCreationRequestId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
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
        public List<TenantCreationRequestItemSpecification> Specifications { get; set; } = new();

        public virtual TenantCreationRequest? TenantCreationRequest { get; set; }
    }
    public class TenantCreationRequestItemSpecification
    {
        public Guid FeatureId { get; set; }
        public string SystemName { get; set; } = string.Empty;
    }
}

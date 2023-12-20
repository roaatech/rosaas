namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantCreationRequest : BaseAuditableEntity
    {
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public decimal SubtotalInclTax { get; set; }
        public decimal SubtotalExclTax { get; set; }
        public decimal Total { get; set; }
        public virtual ICollection<TenantCreationRequestItem> Items { get; set; } = new List<TenantCreationRequestItem>();
    }
}

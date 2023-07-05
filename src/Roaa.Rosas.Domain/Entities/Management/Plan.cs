namespace Roaa.Rosas.Domain.Entities.Management
{
    public class Plan : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public virtual ICollection<PlanFeature> Features { get; set; }
    }
}
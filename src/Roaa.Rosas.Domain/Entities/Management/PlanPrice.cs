namespace Roaa.Rosas.Domain.Entities.Management
{
    public class PlanPrice : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public Cycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual Plan? Plan { get; set; }
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
    }


    public enum Cycle
    {
        Week = 2,
        Month = 3,
        Year = 4,
    }
}
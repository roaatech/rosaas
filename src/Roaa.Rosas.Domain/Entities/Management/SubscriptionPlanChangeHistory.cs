namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionPlanChangeHistory : BaseEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public PlanChangingType Type { get; set; }
        public decimal Price { get; set; }
        public string? Comment { get; set; }
        public DateTime ChangeDate { get; set; }
        public DateTime PlanChangeEnabledDate { get; set; }
        public Guid PlanChangeEnabledByUserId { get; set; }
    }




}

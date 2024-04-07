namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionRenewalHistory : BaseEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle PlanCycle { get; set; }
        public decimal Price { get; set; }
        public string? Comment { get; set; }
        public DateTime RenewalDate { get; set; }
        public DateTime RenewalEnabledDate { get; set; }
        public Guid RenewalEnabledByUserId { get; set; }
        public SubscriptionRenewalTypeEnum Type { get; set; }
    }




}

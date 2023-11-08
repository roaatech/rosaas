namespace Roaa.Rosas.Domain.Entities.Management
{
    public class SubscriptionAutoRenewalHistory : BaseEntity
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
        public Guid SubscriptionId { get; set; }
        public PlanCycle Cycle { get; set; }
        public decimal Price { get; set; }
        public string? Comment { get; set; }
        public DateTime RenewalDate { get; set; }
        public DateTime AutoRenewalEnabledDate { get; set; }
        public Guid AutoRenewalEnabledByUserId { get; set; }
    }

}

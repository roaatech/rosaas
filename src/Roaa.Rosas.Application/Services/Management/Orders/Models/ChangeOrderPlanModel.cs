namespace Roaa.Rosas.Application.Services.Management.Orders.Models
{
    public record ChangeOrderPlanModel
    {
        public Guid PlanId { get; set; }
        public Guid PlanPriceId { get; set; }
    }
}

using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Products.Models
{
    public record ChangeProductTrialTypeModel
    {
        public ProductTrialType TrialType { get; set; }
        public int TrialPeriodInDays { get; set; }
        public Guid? TrialPlanId { get; set; }
        public Guid? TrialPlanPriceId { get; set; }
    }
}

using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record PlanDataModel
{
    public string PlanDisplayName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid PlanPriceId { get; set; }
    public Guid PlanId { get; set; }
    public bool IsPublished { get; set; }
    public PlanCycle PlanCycle { get; set; }
    public Guid GeneratedSubscriptionCycleId { get; set; }
    public Guid GeneratedSubscriptionId { get; set; }
    public TenancyType PlanTenancyType { get; set; }
    public int? CustomPeriodInDays { get; set; } = null;
    public ProductInfoModel Product { get; set; } = new();
    public List<PlanFeatureInfoModel> Features { get; set; } = new();
    public List<SpecificationInfoModel> Specifications { get; set; } = new();
}

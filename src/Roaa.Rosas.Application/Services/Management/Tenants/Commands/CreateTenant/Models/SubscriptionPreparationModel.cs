using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record SubscriptionPreparationModel
{
    public Guid GeneratedSubscriptionCycleId { get; set; }
    public Guid GeneratedSubscriptionId { get; set; }
    public int SequenceNum { get; set; }
    public ProductDataModel Product { get; set; } = new();
    public PlandDataModel Plan { get; set; } = new();
    public PlanPriceDataModel PlanPrice { get; set; } = new();
    public List<PlanFeatureInfoModel> Features { get; set; } = new();
    public List<SpecificationInfoModel> Specifications { get; set; } = new();
    public bool HasTrial { get; set; } = new();
}


public record PlandDataModel
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public TenancyType TenancyType { get; set; }
    public int TrialPeriodInDays { get; set; }
    public bool IsPublished { get; set; }
}

public record PlanPriceDataModel
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public PlanCycle PlanCycle { get; set; }
    public int? CustomPeriodInDays { get; set; } = null;

}

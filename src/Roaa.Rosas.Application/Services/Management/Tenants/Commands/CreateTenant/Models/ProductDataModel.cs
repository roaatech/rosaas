using Roaa.Rosas.Application.Services.Management.Products.Models;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;

public record ProductDataModel : ProductUrlListItem
{
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ProductTrialType TrialType { get; set; }
    public int TrialPeriodInDays { get; set; }
    public Guid? TrialPlanId { get; set; }
    public Guid? TrialPlanPriceId { get; set; }
}

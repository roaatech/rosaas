using MediatR;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public record CreateTenantCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public CreateTenantCommand()
    {
    }

    public List<CreateSubscriptionModel> Subscriptions { get; set; } = new();
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
public record CreateTenantByExternalSystemModel
{
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public record CreateSubscriptionModel
{
    public Guid ProductId { get; set; }
    public Guid PlanId { get; set; }
    public Guid PlanPriceId { get; set; }
    public int? CustomPeriodInDays { get; set; } = null;
    public List<CreateSpecificationValueModel> Specifications { get; set; } = new();
}

public record CreateSpecificationValueModel
{
    public Guid SpecificationId { get; set; }
    public string Value { get; set; } = string.Empty;
}
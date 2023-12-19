using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant;

public record CreateTenantByExternalSystemCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public List<CreateSpecificationValueByExternalSysytemModel> Specifications { get; set; } = new();
    public string TenantName { get; set; } = string.Empty;
    public string TenantDisplayName { get; set; } = string.Empty;
    public string PlanPriceName { get; set; } = string.Empty;
    public int? CustomPeriodInDays { get; set; } = null;
}

public record CreateSpecificationValueByExternalSysytemModel
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
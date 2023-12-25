using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequestByExternalSystem;

public record CreateTenantCreationRequestByExternalSystemCommand : IRequest<Result<TenantCreationRequestByExternalSystemResultDto>>
{
    public List<CreateSpecificationValueByExternalSysytemModel> Specifications { get; set; } = new();
    public string TenantSystemName { get; set; } = string.Empty;
    public string TenantDisplayName { get; set; } = string.Empty;
    public string PlanPriceSystemName { get; set; } = string.Empty;
    public int? CustomPeriodInDays { get; set; } = null;
}

public record CreateSpecificationValueByExternalSysytemModel
{
    public string SystemName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}


public record TenantCreationRequestByExternalSystemResultDto
{
    public string? NavigationUrl { get; set; }
    public TenantCreationRequestByExternalSystemResultDto() { }
    public TenantCreationRequestByExternalSystemResultDto(string navigationUrl)
    {
        NavigationUrl = navigationUrl;
    }
}

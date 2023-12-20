using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpdateTenant;
public record UpdateTenantCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    //  public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
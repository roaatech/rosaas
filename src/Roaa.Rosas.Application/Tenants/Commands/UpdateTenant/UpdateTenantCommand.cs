using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Tenants.Commands.UpdateTenant;
public record UpdateTenantCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string UniqueName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}
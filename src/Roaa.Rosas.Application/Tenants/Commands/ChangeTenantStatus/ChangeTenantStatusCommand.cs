using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

public record ChangeTenantStatusCommand : IRequest<Result<TenantStatusChangedResultDto>>
{
    public ChangeTenantStatusCommand(Guid tenantId, TenantStatus status)
    {
        TenantId = tenantId;
        Status = status;
    }
    public ChangeTenantStatusCommand() { }

    public Guid TenantId { get; init; }
    public TenantStatus Status { get; init; }
}

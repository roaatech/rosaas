using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

public record ChangeTenantStatusByIdCommand : IRequest<Result<List<TenantStatusChangedResultDto>>>
{
    public ChangeTenantStatusByIdCommand(Guid tenantId, TenantStatus status, Guid? productId)
    {
        TenantId = tenantId;
        ProductId = productId;
        Status = status;
    }
    public ChangeTenantStatusByIdCommand() { }

    public Guid TenantId { get; init; }
    public Guid? ProductId { get; init; }
    public TenantStatus Status { get; init; }
}


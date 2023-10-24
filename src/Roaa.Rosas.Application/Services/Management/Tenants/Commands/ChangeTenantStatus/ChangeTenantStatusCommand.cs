using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;

public record ChangeTenantStatusCommand : IRequest<Result<List<TenantStatusChangedResultDto>>>
{
    public ChangeTenantStatusCommand(string tenantName, TenantStatus status, Guid? productId, WorkflowAction action, string notes)
    {
        TenantName = tenantName;
        ProductId = productId;
        Status = status;
        Action = action;
        Notes = notes;
    }

    public ChangeTenantStatusCommand(string tenantName, TenantStatus status, Guid? productId, string notes)
    {
        TenantName = tenantName;
        ProductId = productId;
        Status = status;
        Notes = notes;
    }

    public ChangeTenantStatusCommand() { }

    public string TenantName { get; init; }
    public Guid? ProductId { get; init; }
    public TenantStatus Status { get; init; }
    public WorkflowAction Action { get; init; } = WorkflowAction.Ok;
    public string Notes { get; init; } = string.Empty;
}


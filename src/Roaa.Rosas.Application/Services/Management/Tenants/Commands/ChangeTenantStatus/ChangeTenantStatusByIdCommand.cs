using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;

public record ChangeTenantStatusByIdCommand : IRequest<Result<List<TenantStatusChangedResultDto>>>
{
    public Guid TenantId { get; init; }
    public Guid? ProductId { get; init; }
    public TenantStatus Status { get; init; }
    public WorkflowAction Action { get; init; } = WorkflowAction.Ok;
    public string Comment { get; init; } = string.Empty;


    public ChangeTenantStatusByIdCommand() { }

    public ChangeTenantStatusByIdCommand(Guid tenantId, TenantStatus status, Guid? productId, WorkflowAction action, string comment)
    {
        TenantId = tenantId;
        ProductId = productId;
        Status = status;
        Action = action;
        Comment = comment;
    }

    public ChangeTenantStatusByIdCommand(Guid tenantId, TenantStatus status, Guid? productId, string comment)
    {
        TenantId = tenantId;
        ProductId = productId;
        Status = status;
        Comment = comment;
    }
}


﻿using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus;

public record ChangeTenantStatusByIdCommand : IRequest<Result<List<TenantStatusChangedResultDto>>>
{
    public ChangeTenantStatusByIdCommand(Guid tenantId, TenantStatus status, Guid? productId, WorkflowAction action, string notes)
    {
        TenantId = tenantId;
        ProductId = productId;
        Status = status;
        Action = action;
        Notes = notes;
    }
    public ChangeTenantStatusByIdCommand(Guid tenantId, TenantStatus status, Guid? productId, string notes)
    {
        TenantId = tenantId;
        ProductId = productId;
        Status = status;
        Notes = notes;
    }
    public ChangeTenantStatusByIdCommand() { }

    public Guid TenantId { get; init; }
    public Guid? ProductId { get; init; }
    public TenantStatus Status { get; init; }
    public WorkflowAction Action { get; init; } = WorkflowAction.Ok;
    public string Notes { get; init; } = string.Empty;
}


﻿using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Tenants.Commands.ChangeTenantStatus;

public record TenantStatusChangedResultDto
{
    public TenantStatusChangedResultDto(Guid productId, TenantStatus status, IEnumerable<ActionResultModel> actions)
    {
        ProductId = productId;
        Status = status;
        Actions = actions;
    }
    public TenantStatusChangedResultDto()
    {
    }
    public Guid ProductId { get; set; }
    public TenantStatus Status { get; set; }
    public IEnumerable<ActionResultModel> Actions { get; set; } = new List<ActionResultModel>();
}
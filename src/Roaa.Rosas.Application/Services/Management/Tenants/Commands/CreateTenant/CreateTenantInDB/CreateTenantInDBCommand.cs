﻿using MediatR;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantInDB;

public record CreateTenantInDBCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public CreateTenantInDBCommand()
    {
    }

    public List<TenantCreationPreparationModel> PlanDataList { get; set; } = new();
    public List<CreateSubscriptionModel> Subscriptions { get; set; } = new();
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public UserType UserType { get; set; }
}


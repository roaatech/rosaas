﻿using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.DeleteTenant;


public record DeleteTenantCommand : IRequest<Result>
{
    public Guid TenantId { get; init; }

    public DeleteTenantCommand(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
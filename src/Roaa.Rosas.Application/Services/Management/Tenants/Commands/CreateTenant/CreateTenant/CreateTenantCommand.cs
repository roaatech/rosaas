using MediatR;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenant;

public record CreateTenantCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public CreateTenantCommand()
    {
    }

    public List<SubscriptionPreparationModel> Subscriptions { get; set; } = new();
    public string SystemName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public UserType UserType { get; set; }
    public Guid OrderId { get; set; }
    public Guid TenantRequestId { get; set; }
}


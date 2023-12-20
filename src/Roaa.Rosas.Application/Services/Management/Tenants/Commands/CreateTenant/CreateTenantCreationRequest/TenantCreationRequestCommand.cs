using MediatR;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;

public record TenantCreationRequestCommand : IRequest<Result<TenantCreatedResultDto>>
{
    public TenantCreationRequestCommand()
    {
    }

    public List<CreateSubscriptionModel> Subscriptions { get; set; } = new();
    public string UniqueName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

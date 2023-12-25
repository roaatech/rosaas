using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantCreationRequest;

public record TenantCreationRequestCommand : TenantCreationRequestModel, IRequest<Result<TenantCreationRequestResultDto>>
{
}

public record TenantCreationRequestModel
{
    public List<CreateSubscriptionModel> Subscriptions { get; set; } = new();
    public string? SystemName { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool CreationByOneClick { get; set; }
}

public record TenantCreationRequestResultDto
{
    public string? NavigationUrl { get; set; }
    public bool HasToPay { get; set; }
    public Guid OrderId { get; set; }
    public TenantCreationRequestResultDto() { }
    public TenantCreationRequestResultDto(Guid orderId, bool hasToPay, string navigationUrl)
    {
        NavigationUrl = navigationUrl;
        HasToPay = hasToPay;
        OrderId = orderId;
    }
}

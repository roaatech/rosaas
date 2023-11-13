using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAsDowngradeApplied;
public record SetSubscriptionAsDowngradeAppliedCommand : IRequest<Result>
{
    public string TenantName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public bool IsSuccessful { get; set; }

    public SetSubscriptionAsDowngradeAppliedCommand() { }

    public SetSubscriptionAsDowngradeAppliedCommand(string tenantName, Guid productId, bool isSuccessful)
    {
        TenantName = tenantName;
        ProductId = productId;
        IsSuccessful = isSuccessful;
    }
}
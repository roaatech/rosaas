using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.ApplyDowngradeToSubscription;
public record ApplyDowngradeToSubscriptionCommand : IRequest<Result>
{
    public string TenantName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public bool IsSuccessful { get; set; }

    public ApplyDowngradeToSubscriptionCommand() { }

    public ApplyDowngradeToSubscriptionCommand(string tenantName, Guid productId, bool isSuccessful)
    {
        TenantName = tenantName;
        ProductId = productId;
        IsSuccessful = isSuccessful;
    }
}
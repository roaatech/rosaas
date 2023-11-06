using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscriptionFeatureLimit;
public record ResetSubscriptionFeatureLimitCommand : IRequest<Result>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? SubscriptionFeatureId { get; set; }
    public string Comment { get; init; } = string.Empty;
    public ResetSubscriptionFeatureLimitCommand() { }

    public ResetSubscriptionFeatureLimitCommand(Guid tenantId, Guid productId, Guid? subscriptionFeatureId, string comment)
    {
        TenantId = tenantId;
        ProductId = productId;
        SubscriptionFeatureId = subscriptionFeatureId;
        Comment = comment;
    }
}
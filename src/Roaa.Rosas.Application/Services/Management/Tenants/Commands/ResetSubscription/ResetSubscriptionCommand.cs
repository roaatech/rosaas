using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscription;
public record ResetSubscriptionCommand : IRequest<Result>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public string Comment { get; init; } = string.Empty;


    public ResetSubscriptionCommand() { }

    public ResetSubscriptionCommand(Guid tenantId, Guid productId, string comment)
    {
        TenantId = tenantId;
        ProductId = productId;
        Comment = comment;
    }
}
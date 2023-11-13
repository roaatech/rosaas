using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.UpgradeSubscription;
public record UpgradeSubscriptionCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public Guid PlanId { get; set; }
    public Guid PlanPriceId { get; set; }
    public string? Comment { get; init; }


    public UpgradeSubscriptionCommand() { }

    public UpgradeSubscriptionCommand(Guid subscriptionId, Guid planId, Guid planPriceId, string comment)
    {
        SubscriptionId = subscriptionId;
        PlanId = planId;
        PlanPriceId = planPriceId;
        Comment = comment;
    }
}
using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.PrepareSubscriptionUpgrade;
public record PrepareSubscriptionUpgradeCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public Guid PlanId { get; set; }
    public Guid PlanPriceId { get; set; }
    public string? Comment { get; init; }


    public PrepareSubscriptionUpgradeCommand() { }

    public PrepareSubscriptionUpgradeCommand(Guid subscriptionId, Guid planId, Guid planPriceId, string comment)
    {
        SubscriptionId = subscriptionId;
        PlanId = planId;
        PlanPriceId = planPriceId;
        Comment = comment;
    }
}
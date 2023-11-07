using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAutoRenewal;
public record SetSubscriptionAutoRenewalCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public Guid PlanPriceId { get; set; }
    public string? Comment { get; init; }


    public SetSubscriptionAutoRenewalCommand() { }

    public SetSubscriptionAutoRenewalCommand(Guid subscriptionId, Guid planPriceId, string comment)
    {
        SubscriptionId = subscriptionId;
        PlanPriceId = planPriceId;
        Comment = comment;
    }
}
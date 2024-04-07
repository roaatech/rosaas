using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.CancelSubscriptionRenewal;
public record CancelSubscriptionRenewalCommand : IRequest<Result>
{
    public Guid SubscriptionRenewalId { get; set; }
    public Guid SubscriptionId { get; set; }
    public string? Comment { get; init; }


    public CancelSubscriptionRenewalCommand() { }

    public CancelSubscriptionRenewalCommand(Guid subscriptionRenewalId, Guid subscriptionId, string? comment)
    {
        SubscriptionRenewalId = subscriptionRenewalId;
        SubscriptionId = subscriptionId;
        Comment = comment;
    }
}
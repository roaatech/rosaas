using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.CancelSubscriptionAutoRenewal;
public record CancelSubscriptionAutoRenewalCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public string? Comment { get; init; }


    public CancelSubscriptionAutoRenewalCommand() { }

    public CancelSubscriptionAutoRenewalCommand(Guid subscriptionId, string comment)
    {
        SubscriptionId = subscriptionId;
        Comment = comment;
    }
}
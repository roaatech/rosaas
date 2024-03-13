using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionDowngrade;
public record RequestSubscriptionDowngradeCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public Guid PlanId { get; set; }
    public Guid PlanPriceId { get; set; }
    public string? Comment { get; init; }
    public string CardReferenceId { get; set; }
    public PaymentPlatform PaymentPlatform { get; set; }


    public RequestSubscriptionDowngradeCommand() { }

    public RequestSubscriptionDowngradeCommand(Guid subscriptionId, Guid planId, Guid planPriceId, string cardReferenceId, PaymentPlatform paymentPlatform, string comment)
    {
        SubscriptionId = subscriptionId;
        PlanId = planId;
        PlanPriceId = planPriceId;
        CardReferenceId = cardReferenceId;
        PaymentPlatform = paymentPlatform;
        Comment = comment;
    }
}
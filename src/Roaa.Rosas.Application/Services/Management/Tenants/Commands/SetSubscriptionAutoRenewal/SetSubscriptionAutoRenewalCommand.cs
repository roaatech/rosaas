using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAutoRenewal;
public record SetSubscriptionAutoRenewalCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public string CardReferenceId { get; set; }
    public PaymentPlatform PaymentPlatform { get; set; }
    public Guid? PlanPriceId { get; set; }
    public string? Comment { get; init; }


    public SetSubscriptionAutoRenewalCommand() { }

    public SetSubscriptionAutoRenewalCommand(Guid subscriptionId, string cardReferenceId, PaymentPlatform paymentPlatform, Guid? planPriceId, string comment)
    {
        SubscriptionId = subscriptionId;
        CardReferenceId = cardReferenceId;
        PaymentPlatform = paymentPlatform;
        PlanPriceId = planPriceId;
        Comment = comment;
    }
}
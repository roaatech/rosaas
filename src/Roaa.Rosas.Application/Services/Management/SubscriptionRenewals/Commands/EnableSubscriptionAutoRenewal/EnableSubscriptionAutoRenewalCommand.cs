using MediatR;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionAutoRenewal;
public record EnableSubscriptionAutoRenewalCommand : IRequest<Result>
{
    public Guid SubscriptionId { get; set; }
    public string CardReferenceId { get; set; }
    public PaymentPlatform PaymentPlatform { get; set; }
    public Guid? PlanPriceId { get; set; }
    public int RenewalsCount { get; set; }
    public bool IsContinuousRenewal { get; set; }
    public string? Comment { get; init; }


    public EnableSubscriptionAutoRenewalCommand() { }


    public EnableSubscriptionAutoRenewalCommand(Guid subscriptionId, string cardReferenceId, PaymentPlatform paymentPlatform, Guid? planPriceId, int renewalsCount, bool isContinuousRenewal, string? comment)
    {
        SubscriptionId = subscriptionId;
        CardReferenceId = cardReferenceId;
        PaymentPlatform = paymentPlatform;
        PlanPriceId = planPriceId;
        RenewalsCount = renewalsCount;
        IsContinuousRenewal = isContinuousRenewal;
        Comment = comment;
    }
}
//using MediatR;
//using Roaa.Rosas.Common.Models.Results;
//using Roaa.Rosas.Domain.Enums;

//namespace Roaa.Rosas.Application.Payment.Commands.EnableSubscriptionAutoRenewal;
//public record CapturePaymentCommand : IRequest<Result>
//{
//    public Guid OrderId { get; set; }
//    public PaymentPurpose PaymentPurpose { get; set; }


//    public CapturePaymentCommand(Guid orderId, PaymentPurpose paymentPurpose)
//    {
//        OrderId = orderId;
//        PaymentPurpose = paymentPurpose;
//    }
//}
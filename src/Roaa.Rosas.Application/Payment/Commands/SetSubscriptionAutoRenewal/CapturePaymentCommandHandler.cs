//using MediatR;
//using Microsoft.Extensions.Logging;
//using Roaa.Rosas.Application.Payment.Services;
//using Roaa.Rosas.Authorization.Utilities;
//using Roaa.Rosas.Common.Models.Results;

//namespace Roaa.Rosas.Application.Payment.Commands.EnableSubscriptionAutoRenewal;

//public class CapturePaymentCommandHandler : IRequestHandler<CapturePaymentCommand, Result>
//{
//    #region Props 
//    private readonly ILogger<CapturePaymentCommandHandler> _logger;
//    private readonly IIdentityContextService _identityContextService;
//    private readonly IPaymentService _paymentService;
//    #endregion



//    #region Corts
//    public CapturePaymentCommandHandler(IIdentityContextService identityContextService,
//                                                    IPaymentService paymentService,
//                                                    ILogger<CapturePaymentCommandHandler> logger)
//    {
//        _identityContextService = identityContextService;
//        _paymentService = paymentService;
//        _logger = logger;
//    }
//    #endregion



//    #region Handler   
//    public async Task<Result> Handle(CapturePaymentCommand command, CancellationToken cancellationToken)
//    {
//        return await _paymentService.CapturePaymentAsync(command.OrderId, command.PaymentPurpose, cancellationToken);
//    }
//    #endregion
//}


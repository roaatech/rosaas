using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Payment.Methods;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Payment.Services
{
    public class PaymentService : IPaymentService
    {
        #region Props 
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentMethodFactory _paymentMethodFactory;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IPublisher _publisher;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private IPaymentMethodService? _paymentMethod = null;
        private PaymentMethodType? _paymentMethodType = null;

        #endregion 

        #region Corts
        public PaymentService(ILogger<PaymentService> logger,
                                   IPaymentMethodFactory paymentMethodFactory,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IPublisher publisher,
                                   IOrderService orderService,
                                   IPaymentProcessingService paymentProcessingService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _paymentMethodFactory = paymentMethodFactory;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _publisher = publisher;
            _orderService = orderService;
            _paymentProcessingService = paymentProcessingService;
            _settingService = settingService;
        }
        #endregion



        public async Task<Result<CheckoutResultModel>> CheckoutAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders.Where(x => x.Id == model.OrderId)
                                               .Include(x => x.OrderItems)
                                               .SingleOrDefaultAsync(cancellationToken);
            if (!CanDoPayment(order, true))
            {
                return Result<CheckoutResultModel>.Fail(FetchNonPaymentTransactionReasons(order, model.PaymentMethod), _identityContextService.Locale);
            }

            await _publisher.Publish(new PaymentProcessingPreparationEvent(model.OrderId, model.EnableAutoRenewal));

            var paymentPlatformType = order.OrderTotal == 0 ? PaymentPlatform.Manwal : model.PaymentMethod;

            // TODO : [Determine the payment method (card or other) by the end user]
            var paymentMethodType = paymentPlatformType == PaymentPlatform.Manwal ? PaymentMethodType.None : PaymentMethodType.Card;

            var paymentPlatform = _paymentMethodFactory.GetPaymentMethod(paymentPlatformType);


            var result = await paymentPlatform.CreatePaymentAsync(order, order.OrderItems.Any(x => x.TrialPeriodInDays > 0), model.AllowStoringCardInfo, paymentMethodType, cancellationToken);

            if (!result.Success)
            {
                return Result<CheckoutResultModel>.Fail(result.Messages);
            }

            Guid tenantId = await _dbContext.Tenants.Where(x => x.LastOrderId == order.Id)
                                                 .Select(x => x.Id)
                                                 .FirstOrDefaultAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = result.Data.PaymentLink,
                TenantId = tenantId == Guid.Empty ? null : tenantId,
            });
        }

        public async Task<Result> CapturePaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders.Where(x => x.Id == orderId)
                                               .Include(x => x.OrderItems)
                                               .SingleOrDefaultAsync(cancellationToken);

            if (order.PaymentStatus != PaymentStatus.Authorized)
            {
                return Result.Fail(ErrorMessage.UnauthorizedPaymentCannotBeCaptured, _identityContextService.Locale);
            }

            var paymentPlatform = _paymentMethodFactory.GetPaymentMethod(order.PaymentPlatform);

            return await paymentPlatform.CapturePaymentAsync(order, cancellationToken);
        }



        public bool CanDoPayment(Order order, bool ignoreOrderTotal)
        {

            if (order is null)
            {
                return false;
            }

            if (!ignoreOrderTotal && order.OrderTotal == 0)
            {
                return false;
            }

            if (order.OrderStatus == OrderStatus.Complete)
            {
                return false;
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                return false;
            }

            if (order.IsMustChangePlan)
            {
                return false;
            }

            return true;
        }

        public Enum FetchNonPaymentTransactionReasons(Order order, PaymentPlatform? paymentPlatform)
        {

            if (order is null)
            {
                return CommonErrorKeys.ResourcesNotFoundOrAccessDenied;
            }

            if (order.OrderTotal > 0 && !paymentPlatform.HasValue)
            {
                return ErrorMessage.SelectPaymentPlatform;
            }

            if (order.OrderStatus == OrderStatus.Complete)
            {
                return ErrorMessage.CompletedOrderCannotBeProcessed;
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                return ErrorMessage.CanceledOrderCannotBeProcessed;
            }

            if (order.IsMustChangePlan)
            {
                return ErrorMessage.YouMustSelectPaidPlan;
            }

            throw new NotImplementedException();
        }

    }
}




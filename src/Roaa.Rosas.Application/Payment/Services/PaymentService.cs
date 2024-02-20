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

namespace Roaa.Rosas.Application.Payment.Services
{
    public class PaymentService : IPaymentService
    {
        #region Props 
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentMethodFactory _paymentMethodFactory;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
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
                                   IOrderService orderService,
                                   IPaymentProcessingService paymentProcessingService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _paymentMethodFactory = paymentMethodFactory;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
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

            var paymentMethodType = order.OrderTotal == 0 ? PaymentMethodType.Manwal : model.PaymentMethod;

            var paymentMethod = _paymentMethodFactory.GetPaymentMethod(paymentMethodType);


            var result = await paymentMethod.CreatePaymentAsync(order, order.OrderItems.Any(x => x.TrialPeriodInDays > 0), cancellationToken);

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

            var paymentMethod = _paymentMethodFactory.GetPaymentMethod(order.PaymentMethodType);

            return await paymentMethod.CapturePaymentAsync(order, cancellationToken);
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

        public Enum FetchNonPaymentTransactionReasons(Order order, PaymentMethodType? paymentMethod)
        {

            if (order is null)
            {
                return CommonErrorKeys.ResourcesNotFoundOrAccessDenied;
            }

            if (order.OrderTotal > 0 && !paymentMethod.HasValue)
            {
                return ErrorMessage.SelectPaymentMethod;
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
                return CommonErrorKeys.ResourcesNotFoundOrAccessDenied;
            }

            throw new NotImplementedException();
        }

    }
}




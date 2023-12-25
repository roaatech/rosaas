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



        public async Task<Result<CheckoutResultModel>> HandelPaymentProcessAsyncAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders.Where(x => x.Id == model.OrderId)
                                               .Include(x => x.OrderItems)
                                               .SingleOrDefaultAsync(cancellationToken);
            if (order is null)
            {
                return Result<CheckoutResultModel>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            if (order.OrderTotal > 0 && !model.PaymentMethod.HasValue)
            {
                return Result<CheckoutResultModel>.Fail(ErrorMessage.SelectPaymentMethod, _identityContextService.Locale);
            }

            var paymentMethodType = order.OrderTotal == 0 ? PaymentMethodType.Manwal : model.PaymentMethod;

            var paymentMethod = _paymentMethodFactory.GetPaymentMethod(paymentMethodType);

            await _paymentProcessingService.MarkOrderAsProcessingAsync(order, paymentMethodType, cancellationToken);

            return await paymentMethod.HandelPaymentProcessAsync(order, cancellationToken);
        }
    }
}




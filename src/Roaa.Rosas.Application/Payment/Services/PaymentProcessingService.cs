using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Payment.Methods;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Payment.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        #region Props 
        private readonly ILogger<PaymentProcessingService> _logger;
        private readonly IPaymentMethodFactory _paymentMethodFactory;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private IPaymentMethodService? _paymentMethod = null;
        private PaymentMethodType? _paymentMethodType = null;

        #endregion 

        #region Corts
        public PaymentProcessingService(ILogger<PaymentProcessingService> logger,
                                   IPaymentMethodFactory paymentMethodFactory,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IOrderService orderService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _paymentMethodFactory = paymentMethodFactory;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _orderService = orderService;
            _settingService = settingService;
        }
        #endregion



        public async Task MarkOrderAsProcessingAsync(Order order, PaymentMethodType? paymentMethodType, CancellationToken cancellationToken = default)
        {
            var paymentMethod = _paymentMethodFactory.GetPaymentMethod(paymentMethodType);

            var paymentProcessingExpirationDate = await paymentMethod.GetPaymentProcessingExpirationDate(cancellationToken);

            order.OrderStatus = OrderStatus.Processing;
            order.PaymentStatus = PaymentStatus.Pending;
            order.ModificationDate = DateTime.UtcNow;
            order.PaymentMethodType = paymentMethod.PaymentMethodType;
            order.PaymentProcessingExpirationDate = paymentProcessingExpirationDate;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<Result<CheckoutResultModel>> MarkOrderAsPaidAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.Complete;
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidDate = DateTime.UtcNow;
            order.PayerUserId = _identityContextService.GetActorId();
            order.PayerUserType = _identityContextService.GetUserType();

            order.AddDomainEvent(new OrderPaidEvent(order.Id, order.OrderIntent));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel());
        }
    }
}




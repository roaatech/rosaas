using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Common.SystemMessages;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment
{
    public class PaymentService : IPaymentService
    {


        #region Props 
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentMethodFactory _paymentMethodFactory;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISettingService _settingService;
        private IPaymentMethod? _paymentMethod = null;
        private PaymentMethodType? _paymentMethodType = null;

        #endregion


        #region Corts
        public PaymentService(ILogger<PaymentService> logger,
                                   IPaymentMethodFactory paymentMethodFactory,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _paymentMethodFactory = paymentMethodFactory;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _settingService = settingService;
        }
        #endregion



        public async Task<Result<CheckoutResultModel>> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            //  x.OrderItems.Where(orderItem => orderItem.SubscriptionId == model.SubscriptionId).Any() 
            var order = await _dbContext.Orders.Where(x => x.TenantId == model.TenantId &&
                                                           x.Id == model.OrderId)
                                               .Include(x => x.OrderItems)
                                               .SingleOrDefaultAsync(cancellationToken);
            if (order is null)
            {
                return Result<CheckoutResultModel>.Fail(CommonErrorKeys.ResourcesNotFoundOrAccessDenied, _identityContextService.Locale);
            }

            order.OrderStatus = OrderStatus.Processing;
            order.PaymentStatus = PaymentStatus.Pending;
            order.ModificationDate = DateTime.UtcNow;
            order.PaymentMethodType = model.PaymentMethod;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return await PaymentMethod(model.PaymentMethod ?? PaymentMethodType.Stripe).DoProcessPaymentAsync(order, cancellationToken);
        }


        private IPaymentMethod PaymentMethod(PaymentMethodType? type)
        {
            var paymentMethodType = type ?? PaymentMethodType.Manwal;

            if (_paymentMethod is null || _paymentMethodType is null || _paymentMethodType.Value != paymentMethodType)
            {
                _paymentMethod = _paymentMethodFactory.GetPaymentMethod(paymentMethodType);
            }

            return _paymentMethod;
        }

    }





}




using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Payment.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        #region Props 
        private readonly ILogger<PaymentProcessingService> _logger;
        private readonly IPaymentPlatformFactory _paymentMethodFactory;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Corts
        public PaymentProcessingService(ILogger<PaymentProcessingService> logger,
                                   IPaymentPlatformFactory paymentMethodFactory,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IOrderService orderService,
                                   IGenericAttributeService genericAttributeService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _paymentMethodFactory = paymentMethodFactory;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _orderService = orderService;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
        }
        #endregion





        public async Task<Order> MarkOrderAsProcessingAsync(Order order, PaymentPlatform paymentPlatform, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.PendingToPay;
            order.ModificationDate = DateTime.UtcNow;
            order.PaymentMethodType = paymentMethodType;
            order.PaymentPlatform = paymentPlatform;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<Order> MarkOrderAsAuthorizedAsync(Order order, string cardReferenceId, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.Complete;
            order.PaymentStatus = PaymentStatus.Authorized;

            await SetOrderPayerAsync(order, cancellationToken);

            order.AddDomainEvent(new OrderPaidEvent(order.Id, order.OrderIntent, cardReferenceId, order.PaymentPlatform.Value));

            await _dbContext.SaveChangesAsync(cancellationToken);

            await DeleteCheckoutCreatorAttributeAsync(order, cancellationToken);

            return order;
        }

        public async Task<Order> MarkOrderAsPaidAsync(Order order, string cardReferenceId, PaymentPlatform paymentPlatform, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.Complete;
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidDate = DateTime.UtcNow;

            if (order.PaymentStatus != PaymentStatus.Authorized)
            {
                await SetOrderPayerAsync(order, cancellationToken);
            }

            order.AddDomainEvent(new OrderPaidEvent(order.Id, order.OrderIntent, cardReferenceId, paymentPlatform));

            await _dbContext.SaveChangesAsync(cancellationToken);

            await DeleteCheckoutCreatorAttributeAsync(order, cancellationToken);

            return order;
        }

        public async Task<Order> MarkOrderAsFailedAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.PaymentStatus = PaymentStatus.Failed;

            await SetOrderPayerAsync(order, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await DeleteCheckoutCreatorAttributeAsync(order, cancellationToken);

            return order;
        }








        public async Task SetOrderPayerAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (_identityContextService.IsAuthenticated)
            {
                order.PayerUserId = _identityContextService.GetActorId();
                order.PayerUserType = _identityContextService.GetUserType();
            }
            else
            {
                var checkoutCreator = await _genericAttributeService.GetAttributeAsync<Order, CheckoutCreatorModel>(
                                                      order.Id,
                                                      Consts.GenericAttributeKey.CheckoutCreator,
                                                      null,
                                                      cancellationToken);

                if (checkoutCreator is not null)
                {
                    order.PayerUserId = checkoutCreator.CheckoutCreatedByUserId;
                    order.PayerUserType = checkoutCreator.CheckoutCreatedByUserType;
                }
            }
        }

        public async Task DeleteCheckoutCreatorAttributeAsync(Order order, CancellationToken cancellationToken = default)
        {

            await _genericAttributeService.DeleteAttributeAsync(order, Consts.GenericAttributeKey.CheckoutCreator, cancellationToken);
        }


        public async Task<Order> MarkOrderAsFailedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                       .Where(x => x.Id == orderId)
                                       .SingleOrDefaultAsync(cancellationToken);

            return await MarkOrderAsFailedAsync(order, cancellationToken);
        }
        public async Task<Order> MarkOrderAsAuthorizedAsync(Guid orderId, string cardReferenceId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                       .Where(x => x.Id == orderId)
                                       .SingleOrDefaultAsync(cancellationToken);

            return await MarkOrderAsAuthorizedAsync(order, cardReferenceId, cancellationToken);
        }
        public async Task<Order> MarkOrderAsPaidAsync(Guid orderId, string cardReferenceId, PaymentPlatform paymentPlatform, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                       .Where(x => x.Id == orderId)
                                       .SingleOrDefaultAsync(cancellationToken);

            return await MarkOrderAsPaidAsync(order, cardReferenceId, paymentPlatform, cancellationToken);
        }
    }
}




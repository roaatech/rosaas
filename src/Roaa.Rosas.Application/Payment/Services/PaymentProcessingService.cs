using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Services.Management.Orders;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
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



        public async Task<Order> MarkOrderAsProcessingAsync(Order order, PaymentPlatform paymentPlatform, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.PendingToPay;
            order.ModificationDate = DateTime.UtcNow;
            order.PaymentMethodType = paymentMethodType;
            order.PaymentPlatform = paymentPlatform;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }




        public async Task<Order> MarkOrderAsFailedAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.PaymentStatus = PaymentStatus.Failed;
            order.PayerUserId = _identityContextService.GetActorId();
            order.PayerUserType = _identityContextService.GetUserType();

            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }
        public async Task<Order> MarkOrderAsFailedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                       .Where(x => x.Id == orderId)
                                       .SingleOrDefaultAsync(cancellationToken);

            return await MarkOrderAsFailedAsync(order, cancellationToken);
        }






        public async Task<Order> MarkOrderAsAuthorizedAsync(Order order, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.Complete;
            order.PaymentStatus = PaymentStatus.Authorized;
            order.PayerUserId = _identityContextService.GetActorId();
            order.PayerUserType = _identityContextService.GetUserType();

            order.AddDomainEvent(new OrderPaidEvent(order.Id, order.OrderIntent, order.PaymentMethod?.Card?.ReferenceId, order.PaymentPlatform.Value));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return order;
        }
        public async Task<Order> MarkOrderAsAuthorizedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                       .Where(x => x.Id == orderId)
                                       .SingleOrDefaultAsync(cancellationToken);

            return await MarkOrderAsAuthorizedAsync(order, cancellationToken);
        }





        public async Task<Order> MarkOrderAsPaidAsync(Order order, string cardReferenceId, PaymentPlatform paymentPlatform, CancellationToken cancellationToken = default)
        {
            order.OrderStatus = OrderStatus.Complete;
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidDate = DateTime.UtcNow;
            order.PayerUserId = _identityContextService.GetActorId();
            order.PayerUserType = _identityContextService.GetUserType();

            order.AddDomainEvent(new OrderPaidEvent(order.Id, order.OrderIntent, cardReferenceId, paymentPlatform));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return order;
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




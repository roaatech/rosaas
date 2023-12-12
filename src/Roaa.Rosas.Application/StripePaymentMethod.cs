using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Events.Management;
using Roaa.Rosas.Domain.Settings;
using Stripe.Checkout;

namespace Roaa.Rosas.Application
{
    public class StripePaymentMethod : IPaymentMethod
    {


        #region Props 
        private readonly ILogger<StripePaymentMethod> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly ISettingService _settingService;
        #endregion


        #region Corts
        public StripePaymentMethod(ILogger<StripePaymentMethod> logger,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   ISettingService settingService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _settingService = settingService;
        }
        #endregion



        public async Task<Result<CheckoutResultModel>> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {

            var order = await _dbContext.Orders.Where(x => x.TenantId == model.TenantId &&
                                           x.Id == model.OrderId
                                           //  x.OrderItems.Where(orderItem => orderItem.SubscriptionId == model.SubscriptionId).Any()
                                           )
                                           .Include(x => x.OrderItems)
                                           .SingleOrDefaultAsync(cancellationToken);


            // Create a payment flow from the items in the cart.
            // Gets sent to Stripe API.
            var options = new SessionCreateOptions
            {
                // Stripe calls the URLs below when certain checkout events happen such as success and failure.
                SuccessUrl = $"{_identityContextService.HostUrl}/api/payment/v1/success?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", // Customer paid.
                CancelUrl = $"{_identityContextService.HostUrl}/api/payment/v1/cancel?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", //  Checkout cancelled.
                PaymentMethodTypes = new List<string> // Only card available in test mode?
                {
                    "card"
                },
                Mode = "payment", // One-time payment. Stripe supports recurring 'subscription' payments.
                LineItems = order.OrderItems.Select(OrderItem =>
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {//The Checkout Session's total amount due must add up to at least $0.50 usd

                        UnitAmountDecimal = OrderItem.UnitPriceInclTax * 100, // Price is in USD cents.
                        Currency = order.UserCurrencyCode,

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = OrderItem.Name,
                            Description = OrderItem.DisplayName,
                        },
                    },
                    Quantity = OrderItem.Quantity,

                }).ToList(),
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options, null, cancellationToken);


            order.OrderStatus = Domain.Entities.Management.OrderStatus.Processing;
            order.PaymentStatus = Domain.Entities.Management.PaymentStatus.Pending;
            order.ModificationDate = DateTime.UtcNow;
            order.AuthorizationTransactionId = session.Id;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = session.Url,
            });
        }
        public async Task<Result<CheckoutResultModel>> SuccessAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(sessionId, null, null, cancellationToken);
            if (session.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase) &&
                session.Status.Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                var order = await _dbContext.Orders
                                            .Where(x => x.Id == orderId)
                                            .SingleOrDefaultAsync(cancellationToken);

                order.OrderStatus = Domain.Entities.Management.OrderStatus.Complete;
                order.PaymentStatus = Domain.Entities.Management.PaymentStatus.Paid;
                order.PaidDate = DateTime.UtcNow;
                order.Reference = session.PaymentIntentId;
                order.AuthorizationTransactionResult = JsonConvert.SerializeObject(session);
                order.PayerUserId = _identityContextService.UserId;
                order.PayerUserType = _identityContextService.GetUserType();
                order.AddDomainEvent(new TenantOrderPaidEvent(order.Id, order.TenantId));

                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = "http://localhost:3000/success",
            });
        }


        public async Task<Result<CheckoutResultModel>> CancelAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(sessionId, null, null, cancellationToken);

            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            order.OrderStatus = Domain.Entities.Management.OrderStatus.Pending;
            order.PaymentStatus = Domain.Entities.Management.PaymentStatus.Pending;
            order.Reference = session.PaymentIntentId;
            order.AuthorizationTransactionResult = JsonConvert.SerializeObject(session);


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = "http://localhost:3000/products",
            });
        }
    }



    public class StripeSettings : ISettings
    {
        public string ApiKey { get; set; } = string.Empty;

    }


    public record CheckoutModel
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid OrderId { get; set; }
    }

    public record CheckoutResultModel
    {
        public string NavigationUrl { get; set; } = string.Empty;
    }

}

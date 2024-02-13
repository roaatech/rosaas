using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Options;
using Stripe.Checkout;

namespace Roaa.Rosas.Application.Payment.Methods.StripeService
{
    public class StripePaymentMethodService : IStripePaymentMethodService, IPaymentMethodService
    {


        #region Props 
        private readonly ILogger<StripePaymentMethodService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly ISettingService _settingService;
        private readonly PaymentOptions _appSettings;
        private Session stripeSession;
        #endregion


        #region Corts
        public StripePaymentMethodService(ILogger<StripePaymentMethodService> logger,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IPaymentProcessingService paymentProcessingService,
                                   IApiConfigurationService<PaymentOptions> appSettings,
                                   ISettingService settingService)
        {

            _appSettings = appSettings.Options;
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _paymentProcessingService = paymentProcessingService;
            _settingService = settingService;
        }
        #endregion



        public async Task<Result<CheckoutResultModel>> HandelPaymentProcessAsync(Order order, CancellationToken cancellationToken = default)
        {
            // Create a payment flow from the items in the cart.
            // Gets sent to Stripe API.
            var options = new SessionCreateOptions
            {
                // Stripe calls the URLs below when certain checkout events happen such as success and failure.
                SuccessUrl = $"{_identityContextService.HostUrl}/api/payment/v1/stripe/success?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", // Customer paid.
                CancelUrl = $"{_identityContextService.HostUrl}/api/payment/v1/stripe/cancel?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", //  Checkout cancelled.
                PaymentMethodTypes = new List<string> // Only card available in test mode?
                {
                    "card"
                },
                PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    SetupFutureUsage = "off_session",
                },
                Mode = "payment", // One-time payment. Stripe supports recurring 'subscription' payments.
                Customer = "cus_PXgNESU8G1Giqg",
                LineItems = order.OrderItems.Select(OrderItem =>
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {//The Checkout Session's total amount due must add up to at least $0.50 usd

                        UnitAmountDecimal = OrderItem.UnitPriceInclTax * 100, // Price is in USD cents.
                        Currency = order.UserCurrencyCode,

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = OrderItem.SystemName,
                            Description = OrderItem.DisplayName,
                        },
                    },
                    Quantity = OrderItem.Quantity,

                }).ToList(),
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options, null, cancellationToken);

            order.AuthorizationTransactionId = session.Id;

            await _dbContext.SaveChangesAsync(cancellationToken);

            Guid tenantId = await _dbContext.Tenants.Where(x => x.LastOrderId == order.Id)
                                                      .Select(x => x.Id)
                                                      .FirstOrDefaultAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = session.Url,
                TenantId = tenantId == Guid.Empty ? null : tenantId,
            });
        }



        public async Task<Result<Order>> CompletePaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            order.Reference = stripeSession.PaymentIntentId;
            order.AuthorizationTransactionResult = JsonConvert.SerializeObject(stripeSession);

            await _paymentProcessingService.MarkOrderAsPaidAsync(order);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<Order>.Successful(order);
        }



        public async Task<Result<CheckoutResultModel>> SuccessAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var sessionService = new SessionService();
            stripeSession = await sessionService.GetAsync(sessionId, null, null, cancellationToken);

            if (stripeSession.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase) &&
                stripeSession.Status.Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                await CompletePaymentProcessAsync(orderId, cancellationToken);
            }

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = _appSettings.SuccessPageUrl,
            });
        }



        public async Task<Result<CheckoutResultModel>> CancelAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(sessionId, null, null, cancellationToken);

            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            order.OrderStatus = OrderStatus.Initial;
            order.PaymentStatus = PaymentStatus.Initial;
            order.Reference = session.PaymentIntentId;
            order.AuthorizationTransactionResult = JsonConvert.SerializeObject(session);


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = _appSettings.CancelPageUrl,
            });
        }



        public async Task<DateTime> GetPaymentProcessingExpirationDate(CancellationToken cancellationToken = default)
        {
            return DateTime.UtcNow.AddMinutes(5);
        }



        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Stripe;
            }
        }
    }
}







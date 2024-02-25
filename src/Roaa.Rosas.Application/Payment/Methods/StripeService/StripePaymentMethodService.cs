using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.Rosas.Application.Constatns;
using Roaa.Rosas.Application.IdentityContextUtilities;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Identity.Accounts.Queries.GetUserProfileByUserId;
using Roaa.Rosas.Application.Services.Management.GenericAttributes;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Options;
using Stripe;
using Stripe.Checkout;

namespace Roaa.Rosas.Application.Payment.Methods.StripeService
{
    public class StripePaymentMethodService : IStripePaymentMethodService, IPaymentMethodService
    {


        #region Props 
        private readonly ILogger<StripePaymentMethodService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly ISettingService _settingService;
        private readonly PaymentOptions _appSettings;
        private readonly ISender _mediatR;
        private Session stripeSession;
        #endregion


        #region Corts
        public StripePaymentMethodService(ILogger<StripePaymentMethodService> logger,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IPaymentProcessingService paymentProcessingService,
                                   IGenericAttributeService genericAttributeService,
                                   IApiConfigurationService<PaymentOptions> appSettings,
                                   ISettingService settingService,
                                   ISender mediatR)
        {

            _appSettings = appSettings.Options;
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _paymentProcessingService = paymentProcessingService;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
            _mediatR = mediatR;
        }
        #endregion

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Stripe;
            }
        }




        #region Stripe Utilities
        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId, null, null, cancellationToken);
        }

        public async Task<Customer> CreateCustomerAsync(string email, string name, string phone, Guid userId, CancellationToken cancellationToken = default)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Phone = phone,
                Metadata = new Dictionary<string, string>
                {
                    { Consts.StripeDefaults.RoSaasUserId,userId.ToString() }
                },
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options);

            await _genericAttributeService.SaveAttributeAsync<User, string?>(
                                                        _identityContextService.UserId,
                                                        Consts.GenericAttributeKey.StripeCustomerId,
                                                        customer.Id,
                                                        cancellationToken);
            return customer;
        }

        public async Task UpdateCustomerAsync(string name, string phone, Guid userId, CancellationToken cancellationToken = default)
        {
            var stripeCustomerId = await _genericAttributeService.GetAttributeAsync<User, string?>(
                                                      userId,
                                                      Consts.GenericAttributeKey.StripeCustomerId,
                                                      null,
                                                      cancellationToken);
            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                return;
            }

            var options = new CustomerUpdateOptions
            {
                Name = name,
                Phone = phone,
            };
            var service = new CustomerService();

            var nCustomer = await service.UpdateAsync(stripeCustomerId, options, null, cancellationToken);

            return;
        }

        public async Task<string> FeatchCurrentCustomerIdAsync(CancellationToken cancellationToken = default)
        {
            if (!_identityContextService.IsTenantAdmin())
            {
                return null;
            }

            var stripeCustomerId = await _genericAttributeService.GetAttributeAsync<User, string?>(
                                                        _identityContextService.UserId,
                                                        Consts.GenericAttributeKey.StripeCustomerId,
                                                        null,
                                                        cancellationToken);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                var result = await _mediatR.Send(new GetUserProfileByUserIdQuery(_identityContextService.UserId));

                if (result.Success)
                {

                    var customer = await CreateCustomerAsync(result.Data.UserAccount.Email,
                                                             result.Data.UserProfile.FullName,
                                                             result.Data.UserProfile.MobileNumber,
                                                             _identityContextService.UserId);
                    stripeCustomerId = customer.Id;
                }
            }

            return stripeCustomerId;
        }

        public async Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new SessionService();

                return await service.GetAsync(sessionId, null, null, cancellationToken);
            }
            catch (StripeException e)
            {
                switch (e.StripeError.Code)
                {
                    case "resource_missing":
                        return null;
                    default:
                        throw;
                }
            }

        }

        public async Task<Session> CreateSessionPaymentAsync(Order order, bool isCaptureMethod, bool storeCardInfo = true, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(order.AltProcessedPaymentId))
            {
                Session _session = await GetSessionAsync(order.AltProcessedPaymentId, cancellationToken);
                if (_session is not null &&
                    _session.ExpiresAt > DateTime.UtcNow.AddMinutes(5) &&
                    _session.Status.Equals(Consts.StripeDefaults.SessionPendingStatus, StringComparison.OrdinalIgnoreCase))
                {
                    return _session;
                }
            }

            // Create a payment flow from the items in the cart.
            // Gets sent to Stripe API.
            var options = new SessionCreateOptions
            {
                // Stripe calls the URLs below when certain checkout events happen such as success and failure.
                SuccessUrl = $"{_identityContextService.HostUrl}/api/payment/v1/stripe/session/success?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", // Customer paid.
                CancelUrl = $"{_identityContextService.HostUrl}/api/payment/v1/stripe/session/failed?sessionId=" + "{CHECKOUT_SESSION_ID}&orderId=" + $"{order.Id}", //  Checkout cancelled.
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
                            Name = OrderItem.SystemName,
                            Description = OrderItem.DisplayName,
                        },
                    },
                    Quantity = OrderItem.Quantity,

                }).ToList(),
            };



            options.Metadata = new Dictionary<string, string> { { Consts.StripeDefaults.RoSaasOrderId, order.Id.ToString() } };

            if (storeCardInfo)
            {
                options.PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    SetupFutureUsage = "off_session",
                };
            }

            if (_identityContextService.IsTenantAdmin())
            {
                options.Customer = await FeatchCurrentCustomerIdAsync(cancellationToken);
            }

            if (isCaptureMethod)
            {
                options.PaymentIntentData = new Stripe.Checkout.SessionPaymentIntentDataOptions
                {
                    CaptureMethod = "manual",
                };
            }



            var service = new SessionService();
            Session session = await service.CreateAsync(options, null, cancellationToken);
            return session;
        }

        public async Task<PaymentIntent> CaptureFundsAsync(string paymentIntentId, long amount, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCaptureOptions
            {
                AmountToCapture = amount,
            };
            return service.Capture(paymentIntentId, options);
        }
        #endregion




        #region Services
        public async Task<Result<PaymentMethodCheckoutResultModel>> CreatePaymentAsync(Order order, bool setAuthorizedPayment, CancellationToken cancellationToken = default)
        {
            Session session = await CreateSessionPaymentAsync(order, setAuthorizedPayment, true, cancellationToken);

            await _paymentProcessingService.MarkOrderAsProcessingAsync(order, PaymentMethodType, cancellationToken);

            order.AltProcessedPaymentId = session.Id;
            order.ProcessedPaymentReferenceType = session.GetType().Name;
            order.ProcessedPaymentReference = JsonConvert.SerializeObject(session);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<PaymentMethodCheckoutResultModel>.Successful(new PaymentMethodCheckoutResultModel
            {
                PaymentLink = session.Url,
            });
        }

        public async Task<Result> CapturePaymentAsync(Order order, CancellationToken cancellationToken = default)
        {
            var paymentIntentId = order.ProcessedPaymentId;
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId, null, null, cancellationToken);

            paymentIntent = await CaptureFundsAsync(paymentIntentId, paymentIntent.Amount, cancellationToken);

            order = await _paymentProcessingService.MarkOrderAsPaidAsync(order, cancellationToken);

            order.ProcessedPaymentId = paymentIntent.Id;
            order.ProcessedPaymentResult = paymentIntent.Status;
            order.ProcessedPaymentReferenceType = paymentIntent.GetType().Name;
            order.ProcessedPaymentReference = JsonConvert.SerializeObject(paymentIntent);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Successful();
        }

        public async Task<Result<Order>> CompleteSuccessfulPaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            order.ProcessedPaymentReference = stripeSession.PaymentIntentId;
            order.AuthorizedPaymentResult = JsonConvert.SerializeObject(stripeSession);



            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<Order>.Successful(order);
        }

        public async Task<Result<CheckoutResultModel>> CompleteSuccessfulSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var session = await GetSessionAsync(sessionId, cancellationToken);

            var paymentIntent = await GetPaymentIntentAsync(session.PaymentIntentId, cancellationToken);

            Order order = null;

            switch (paymentIntent.Status)
            {
                // Paid Payment
                case Consts.StripeDefaults.PaymentIntentPaidStatus:
                    order = await _paymentProcessingService.MarkOrderAsPaidAsync(orderId, cancellationToken);
                    order.ProcessedPaymentResult = paymentIntent.Status;
                    break;
                // Authorized Payment
                case Consts.StripeDefaults.PaymentIntentAuthorizedStatus:
                    order = await _paymentProcessingService.MarkOrderAsAuthorizedAsync(orderId, cancellationToken);
                    order.AuthorizedPaymentResult = paymentIntent.Status;
                    break;
                default:
                    order = await _paymentProcessingService.MarkOrderAsFailedAsync(orderId, cancellationToken);
                    order.ProcessedPaymentResult = paymentIntent.Status;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
                    {
                        NavigationUrl = _appSettings.CancelPageUrl,
                    });
            }


            order.ProcessedPaymentId = paymentIntent.Id;
            order.ProcessedPaymentReferenceType = paymentIntent.GetType().Name;
            order.ProcessedPaymentReference = JsonConvert.SerializeObject(paymentIntent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = _appSettings.SuccessPageUrl,
            });
        }

        public async Task<Result<CheckoutResultModel>> CompleteFailedSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var session = await GetSessionAsync(sessionId, cancellationToken);

            var order = await _paymentProcessingService.MarkOrderAsFailedAsync(orderId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(session.PaymentIntentId))
            {
                var paymentIntent = await GetPaymentIntentAsync(session.PaymentIntentId, cancellationToken);
                order.ProcessedPaymentId = paymentIntent.Id;
                order.ProcessedPaymentReferenceType = paymentIntent.GetType().Name;
                order.ProcessedPaymentReference = JsonConvert.SerializeObject(paymentIntent);
            }
            else
            {
                order.ProcessedPaymentReferenceType = session.GetType().Name;
                order.ProcessedPaymentReference = JsonConvert.SerializeObject(session);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultModel>.Successful(new CheckoutResultModel
            {
                NavigationUrl = _appSettings.CancelPageUrl,
            });
        }
        #endregion
    }
}







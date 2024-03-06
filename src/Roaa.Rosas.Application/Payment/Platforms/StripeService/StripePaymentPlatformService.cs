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
using Roaa.Rosas.Application.SystemMessages;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Identity;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models.Options;
using Roaa.Rosas.Domain.Models.Payment;
using Stripe;
using Stripe.Checkout;

namespace Roaa.Rosas.Application.Payment.Platforms.StripeService
{
    public class StripePaymentPlatformService : IStripePaymentPlatformService, IPaymentPlatformService
    {


        #region Props 
        private readonly ILogger<StripePaymentPlatformService> _logger;
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
        public StripePaymentPlatformService(ILogger<StripePaymentPlatformService> logger,
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

        public PaymentPlatform PaymentPlatform
        {
            get
            {
                return PaymentPlatform.Stripe;
            }
        }



        #region   Utilities 
        private async Task<string> FeatchStripeCustomerIdFromStoreAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _genericAttributeService.GetAttributeAsync<User, string>(
                                                       userId,
                                                       Consts.GenericAttributeKey.StripeCustomerId,
                                                       string.Empty,
                                                       cancellationToken);
        }
        #endregion


        #region Stripe Utilities

        private async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId, null, null, cancellationToken);
        }
        private async Task<StripeList<Customer>> GetCustomersListByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var service = new CustomerService();
            var options = new CustomerListOptions
            {
                Email = email,
            };
            return await service.ListAsync(options, null, cancellationToken);
        }
        private async Task<Customer> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var service = new CustomerService();
            return await service.GetAsync(customerId, null, null, cancellationToken);
        }
        private async Task<Customer> CreateCustomerAsync(string email, string name, string phone, Guid userId, CancellationToken cancellationToken = default)
        {
            var service = new CustomerService();
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
            return await service.CreateAsync(options);
        }
        private async Task<Customer> UpdateCustomerAsync(string customerId, string name, string phone, CancellationToken cancellationToken = default)
        {
            var service = new CustomerService();
            var options = new CustomerUpdateOptions
            {
                Name = name,
                Phone = phone,
            };
            return await service.UpdateAsync(customerId, options, null, cancellationToken);

        }
        private async Task AttachPaymentMethodAsync(string customerId, string cardId)
        {
            var service = new PaymentMethodService();
            await service.AttachAsync(cardId, new PaymentMethodAttachOptions { Customer = customerId });
        }
        private async Task DetachPaymentMethodAsync(string cardId)
        {
            var service = new PaymentMethodService();
            await service.DetachAsync(cardId);
        }
        private Task<StripeList<Stripe.PaymentMethod>> GetCustomerPaymentMethodCardsListAsync(string paymentMethodId, CancellationToken cancellationToken = default)
        {
            var service = new CustomerService();
            return service.ListPaymentMethodsAsync(paymentMethodId, new CustomerListPaymentMethodsOptions { Type = "card" }, null, cancellationToken);
        }
        private async Task<Stripe.PaymentMethod> GetPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default)
        {
            var service = new PaymentMethodService();
            return await service.GetAsync(paymentMethodId, null, null, cancellationToken);
        }
        private async Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
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
        private async Task<Customer> CreateCustomerIfNotExistsAsync(string email, string name, string phone, Guid userId, CancellationToken cancellationToken = default)
        {
            Customer customer = null;

            var customersList = await GetCustomersListByEmailAsync(email, cancellationToken);

            if (customersList.Any())
            {
                customer = customersList.First();
            }
            else
            {
                customer = await CreateCustomerAsync(email, name, phone, userId, cancellationToken);
            }

            await _genericAttributeService.SaveAttributeAsync<User, string?>(
                                                        _identityContextService.UserId,
                                                        Consts.GenericAttributeKey.StripeCustomerId,
                                                        customer.Id,
                                                        cancellationToken);
            return customer;
        }
        private async Task<Customer> MarkPaymentMethodAsDefaultAsync(string customerId, string stripeCardId, CancellationToken cancellationToken = default)
        {
            var options = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = stripeCardId,
                }
            };
            var service = new CustomerService();

            var nCustomer = await service.UpdateAsync(customerId, options, null, cancellationToken);

            return nCustomer;
        }
        private async Task<string> FeatchCustomerIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            if (!_identityContextService.IsTenantAdmin())
            {
                return null;
            }


            var stripeCustomerId = await FeatchStripeCustomerIdFromStoreAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                var result = await _mediatR.Send(new GetUserProfileByUserIdQuery(userId));

                if (result.Success)
                {

                    var customer = await CreateCustomerIfNotExistsAsync(result.Data.UserAccount.Email,
                                                                         result.Data.UserProfile.FullName,
                                                                         result.Data.UserProfile.MobileNumber,
                                                                         userId);
                    stripeCustomerId = customer.Id;
                }
            }

            return stripeCustomerId;
        }
        private async Task<Session> CreateSessionPaymentAsync(Order order, bool isCaptureMethod, bool storeCardInfo = true, CancellationToken cancellationToken = default)
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



            if (_identityContextService.IsTenantAdmin())
            {
                options.Customer = await FeatchCustomerIdAsync(_identityContextService.UserId, cancellationToken);

                if (storeCardInfo)
                {
                    options.PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        SetupFutureUsage = "on_session",
                    };
                }
            }

            if (isCaptureMethod)
            {
                options.PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    CaptureMethod = "manual",
                };
            }



            var service = new SessionService();
            Session session = await service.CreateAsync(options, null, cancellationToken);
            return session;
        }
        private async Task<PaymentIntent> CaptureFundsAsync(string paymentIntentId, long amount, CancellationToken cancellationToken = default)
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
        public async Task<Result> MarkPaymentMethodAsDefaultAsync(Guid userId, string stripeCardId, CancellationToken cancellationToken = default)
        {
            var stripeCustomerId = await FeatchStripeCustomerIdFromStoreAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                return Result.Fail(ErrorMessage.StripeCustomerNotExist, _identityContextService.Locale);
            }

            await MarkPaymentMethodAsDefaultAsync(stripeCustomerId, stripeCardId);

            return Result.Successful();
        }
        public async Task UpdateCustomerAsync(string name, string phone, Guid userId, CancellationToken cancellationToken = default)
        {
            var stripeCustomerId = await FeatchStripeCustomerIdFromStoreAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                return;
            }

            await UpdateCustomerAsync(stripeCustomerId, name, phone, cancellationToken);

            return;
        }
        public async Task<Result> AttachPaymentMethodCardAsync(Guid userId, string stripeCardId, CancellationToken cancellationToken = default)
        {
            var stripeCustomerId = await FeatchStripeCustomerIdFromStoreAsync(userId, cancellationToken);


            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                stripeCustomerId = await FeatchCustomerIdAsync(_identityContextService.UserId, cancellationToken);
            }

            await AttachPaymentMethodAsync(stripeCustomerId, stripeCardId);

            return Result.Successful();
        }

        public async Task<Result> DetachPaymentMethodCardAsync(string stripeCardId, CancellationToken cancellationToken = default)
        {

            await DetachPaymentMethodAsync(stripeCardId);

            return Result.Successful();
        }
        public async Task<Result<List<PaymentMethodCardListItem>>> GetPaymentMethodsCardsListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var stripeCustomerId = await FeatchStripeCustomerIdFromStoreAsync(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                return Result<List<PaymentMethodCardListItem>>.Successful(new List<PaymentMethodCardListItem>());
            }

            var stripeResults = await GetCustomerPaymentMethodCardsListAsync(stripeCustomerId, cancellationToken);

            var customer = await GetCustomerAsync(stripeCustomerId, cancellationToken);

            string defaultPaymentMethodId = customer.InvoiceSettings.DefaultPaymentMethodId ?? string.Empty;


            var cards = stripeResults.Select(x => new PaymentMethodCardListItem
            {
                StripeCardId = x.Id,
                ReferenceId = x.Id,
                Brand = x.Card.Brand,
                ExpirationMonth = (int)x.Card.ExpMonth,
                ExpirationYear = (int)x.Card.ExpYear,
                Last4Digits = x.Card.Last4,
                CardholderName = x.BillingDetails.Name,
                IsDefault = string.Equals(x.Id, defaultPaymentMethodId),
                PaymentPlatform = PaymentPlatform.Stripe,
            }).ToList();

            return Result<List<PaymentMethodCardListItem>>.Successful(cards);
        }
        public async Task<Result<PaymentMethodCheckoutResultModel>> CreatePaymentAsync(Order order, bool setAuthorizedPayment, bool storeCardInfo, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default)
        {
            Session session = await CreateSessionPaymentAsync(order, setAuthorizedPayment, storeCardInfo, cancellationToken);

            await _paymentProcessingService.MarkOrderAsProcessingAsync(order, PaymentPlatform, paymentMethodType, cancellationToken);

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

            order = await _paymentProcessingService.MarkOrderAsPaidAsync(order, paymentIntent.PaymentMethodId, PaymentPlatform, cancellationToken);

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
                    order = await _paymentProcessingService.MarkOrderAsPaidAsync(orderId, paymentIntent.PaymentMethodId, PaymentPlatform, cancellationToken);
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

            var paymentMethod = await GetPaymentMethodAsync(paymentIntent.PaymentMethodId, cancellationToken);

            if (paymentMethod != null)
            {
                order.PaymentMethod = new Domain.Entities.Management.PaymentMethod
                {
                    Card = new Domain.Entities.Management.PaymentMethodCard
                    {
                        ReferenceId = paymentMethod.Id,
                        Brand = paymentMethod.Card.Brand,
                        ExpirationMonth = (int)paymentMethod.Card.ExpMonth,
                        ExpirationYear = (int)paymentMethod.Card.ExpYear,
                        Last4Digits = paymentMethod.Card.Last4,
                        CardholderName = paymentMethod.BillingDetails.Name,
                    }
                };
            }
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







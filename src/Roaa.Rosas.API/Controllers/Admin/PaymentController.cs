using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Payment.Platforms.StripeService;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route("api/payment/v1")]
    [Authorize(Policy = AuthPolicy.Identity.Account, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class PaymentController : BaseRosasApiController
    {
        #region Props 
        private readonly ILogger<AuthController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityContextService _identityContextService;
        private readonly IStripePaymentPlatformService _stripePaymentMethod;
        #endregion

        #region Corts

        public PaymentController(ILogger<AuthController> logger,
                                  IWebHostEnvironment environment,
                                  IPaymentService paymentService,
                                  IIdentityContextService identityContextService,
                                  IStripePaymentPlatformService stripePaymentMethod)
        {
            _logger = logger;
            _environment = environment;
            _paymentService = paymentService;
            _identityContextService = identityContextService;
            _stripePaymentMethod = stripePaymentMethod;
        }
        #endregion


        #region Actions   

        [AllowAnonymous]
        [HttpPost("Checkout")]
        public async Task<IActionResult> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var result = await _paymentService.CheckoutAsync(model, cancellationToken);

            return ItemResult(result);
        }



        [HttpGet("Cards")]
        public async Task<IActionResult> GetPaymentMethodsCardsListAsync(CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.GetPaymentMethodsCardsListByUserIdAsync(_identityContextService.UserId, cancellationToken);

            return ListResult(result);
        }




        [HttpPost("Cards/{stripeCardId}")]
        public async Task<IActionResult> AttachPaymentMethodCardAsync(string stripeCardId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.AttachPaymentMethodCardAsync(_identityContextService.UserId, stripeCardId, cancellationToken);

            return EmptyResult(result);
        }



        [HttpDelete("Cards/{stripeCardId}")]
        public async Task<IActionResult> DetachPaymentMethodCardAsync(string stripeCardId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.DetachPaymentMethodCardAsync(stripeCardId, cancellationToken);

            return EmptyResult(result);
        }



        [HttpPost("Cards/{stripeCardId}/Default")]
        public async Task<IActionResult> MarkPaymentMethodAsDefaultAsync(string stripeCardId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.MarkPaymentMethodAsDefaultAsync(_identityContextService.UserId, stripeCardId, cancellationToken);

            return EmptyResult(result);
        }





        #endregion

    }
}

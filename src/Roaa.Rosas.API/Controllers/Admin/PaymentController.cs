using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application;
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
        private readonly IPaymentMethod _paymentMethod;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts

        public PaymentController(ILogger<AuthController> logger,
                                  IWebHostEnvironment environment,
                                  IPaymentMethod paymentMethod)
        {
            _logger = logger;
            _environment = environment;
            _paymentMethod = paymentMethod;
        }
        #endregion


        #region Actions   

        [HttpPost("Checkout")]
        public async Task<IActionResult> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var result = await _paymentMethod.ProcessPaymentAsync(model, cancellationToken);

            return ItemResult(result);
        }

        [AllowAnonymous]
        [HttpGet("success")]
        public async Task<IActionResult> SuccessAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var result = await _paymentMethod.SuccessAsync(sessionId, orderId, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }


        [AllowAnonymous]
        [HttpGet("failed")]
        public async Task<IActionResult> FailedAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var result = await _paymentMethod.CancelAsync(sessionId, orderId, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }

        #endregion

    }
}

﻿using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Payment;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Route("api/payment/v1")]
    [Authorize(Policy = AuthPolicy.Identity.Account, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class StripePaymentMethodController : BaseRosasApiController
    {
        #region Props 
        private readonly ILogger<AuthController> _logger;
        private readonly IStripePaymentMethod _stripePaymentMethod;
        #endregion

        #region Corts

        public StripePaymentMethodController(ILogger<AuthController> logger,
                                  IStripePaymentMethod stripePaymentMethod)
        {
            _logger = logger;
            _stripePaymentMethod = stripePaymentMethod;
        }
        #endregion


        #region Actions   


        [AllowAnonymous]
        [HttpGet("stripe/success")]
        public async Task<IActionResult> SuccessHandlerAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.SuccessAsync(sessionId, orderId, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }


        [AllowAnonymous]
        [HttpGet("stripe/failed")]
        public async Task<IActionResult> FailedHandlerAAsync(string sessionId, Guid? orderId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.CancelAsync(sessionId, orderId, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }

        #endregion

    }
}

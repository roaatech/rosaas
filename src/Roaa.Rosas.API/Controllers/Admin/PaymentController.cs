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
    public class PaymentController : BaseRosasApiController
    {
        #region Props 
        private readonly ILogger<AuthController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Corts

        public PaymentController(ILogger<AuthController> logger,
                                  IWebHostEnvironment environment,
                                  IPaymentService paymentService)
        {
            _logger = logger;
            _environment = environment;
            _paymentService = paymentService;
        }
        #endregion


        #region Actions   

        [HttpPost("Checkout")]
        public async Task<IActionResult> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var result = await _paymentService.ProcessPaymentAsync(model, cancellationToken);

            return ItemResult(result);
        }

        #endregion

    }
}

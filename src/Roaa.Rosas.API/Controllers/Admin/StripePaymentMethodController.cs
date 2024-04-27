using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Payment.Platforms.StripeService;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CreateTenant.CreateTenantByOrder;
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
        private readonly ISender _mediator;
        private readonly IStripePaymentPlatformService _stripePaymentMethod;
        #endregion

        #region Corts

        public StripePaymentMethodController(ILogger<AuthController> logger,
                                             ISender mediator,
                                             IStripePaymentPlatformService stripePaymentMethod)
        {
            _logger = logger;
            _stripePaymentMethod = stripePaymentMethod;
            _mediator = mediator;
        }
        #endregion


        #region Actions   


        [AllowAnonymous]
        [HttpGet("stripe/session/success")]
        public async Task<IActionResult> CompleteSuccessfulSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.CompleteSuccessfulSessionPaymentAsync(sessionId, orderId, cancellationToken);

            await _mediator.Send(new CreateTenantByOrderCommand
            {
                OrderId = result.Data.Order.Id,
                CardReferenceId = result.Data.Order.PaymentMethod!.Card!.ReferenceId,
                PaymentPlatform = Domain.Entities.Management.PaymentPlatform.Stripe,
            }, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }


        [AllowAnonymous]
        [HttpGet("stripe/session/failed")]
        public async Task<IActionResult> CompleteFailedSessionPaymentAsync(string sessionId, Guid orderId, CancellationToken cancellationToken = default)
        {
            var result = await _stripePaymentMethod.CompleteFailedSessionPaymentAsync(sessionId, orderId, cancellationToken);

            Response.Headers.Add("Location", result.Data.NavigationUrl);

            return new StatusCodeResult(303);
        }




        #endregion

    }
}

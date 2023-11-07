using MediatR;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.CancelSubscriptionAutoRenewal;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscription;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscriptionFeatureLimit;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.SetSubscriptionAutoRenewal;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    public class SubscriptionsController : BaseSuperAdminMainApiController
    {
        #region Props 
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly ITenantService _tenantService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public SubscriptionsController(ILogger<SubscriptionsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                                ITenantService tenantService,
                                ISender mediator)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _mediator = mediator;
        }
        #endregion

        #region Actions   


        [HttpPost("Reset")]
        public async Task<IActionResult> ResetSubscriptionAsync([FromBody] ResetSubscriptionCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("Features/Reset")]
        public async Task<IActionResult> ResetSubscriptionFeaturesLimitsAsync([FromBody] ResetSubscriptionFeatureLimitCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("AutoRenewal")]
        public async Task<IActionResult> SetSubscriptionAutoRenewalAsync([FromBody] SetSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpDelete("AutoRenewal")]
        public async Task<IActionResult> CancelSubscriptionAutoRenewalAsync([FromBody] CancelSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }

        #endregion 
    }
}

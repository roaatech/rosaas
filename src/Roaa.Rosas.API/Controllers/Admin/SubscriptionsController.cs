using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.CancelSubscriptionRenewal;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Commands.EnableSubscriptionAutoRenewal;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Queries.GetSubscriptionAutoRenewalsList;
using Roaa.Rosas.Application.Services.Management.Subscriptions;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Queries.GetSubscriptionCycles;
using Roaa.Rosas.Application.Services.Management.Subscriptions.Queries.GetSubscriptionsList;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.PrepareSubscriptionReset;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionDowngrade;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.RequestSubscriptionUpgrade;
using Roaa.Rosas.Application.Services.Management.Tenants.Commands.ResetSubscriptionFeatureLimit;
using Roaa.Rosas.Application.Services.Management.Tenants.Service;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Framework.Controllers.Common;

namespace Roaa.Rosas.Framework.Controllers.Admin
{
    [Authorize(Policy = AuthPolicy.Management.Subscriptions, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class SubscriptionsController : BaseManagementApiController
    {
        #region Props 
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly ITenantService _tenantService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IIdentityContextService _identityContextService;
        private readonly IWebHostEnvironment _environment;
        private readonly ISubscriptionRenewalService _subscriptionRenewalService;
        private readonly ISender _mediator;
        #endregion

        #region Corts
        public SubscriptionsController(ILogger<SubscriptionsController> logger,
                                IWebHostEnvironment environment,
                                IIdentityContextService identityContextService,
                                ITenantService tenantService,
                                ISubscriptionService subscriptionService,
                                ISubscriptionRenewalService subscriptionRenewalService,
                                ISender mediator)
        {
            _logger = logger;
            _environment = environment;
            _identityContextService = identityContextService;
            _tenantService = tenantService;
            _subscriptionService = subscriptionService;
            _subscriptionRenewalService = subscriptionRenewalService;
            _mediator = mediator;
        }
        #endregion

        #region Actions   
        [HttpGet()]
        public async Task<IActionResult> GetSubscriptionsListAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetSubscriptionsListQuery(), cancellationToken));
        }


        [HttpPost("Reset")]
        public async Task<IActionResult> ResetSubscriptionAsync([FromBody] PrepareSubscriptionResetCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("Features/Reset")]
        public async Task<IActionResult> ResetSubscriptionFeaturesLimitsAsync([FromBody] ResetSubscriptionFeatureLimitCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("AutoRenewal")]
        public async Task<IActionResult> EnableSubscriptionAutoRenewalAsync([FromBody] EnableSubscriptionAutoRenewalCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }

        [Obsolete("This Action is obsolete. Use Renewals instead.", false)]
        [HttpGet("AutoRenewal")]
        public async Task<IActionResult> GetSubscriptionAutoRenewalsListByUserIdAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetSubscriptionRenewalsListQuery(), cancellationToken));
        }

        [HttpGet("Renewals")]
        public async Task<IActionResult> GetSubscriptionRenewalsListByUserIdAsync(CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetSubscriptionRenewalsListQuery(), cancellationToken));
        }


        [Obsolete("This Action is obsolete. Use Renewals instead.", false)]
        [HttpDelete("AutoRenewal")]
        public async Task<IActionResult> CancelSubscriptionAutoRenewalAsync([FromBody] CancelSubscriptionRenewalCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }

        [HttpDelete("Renewals")]
        public async Task<IActionResult> CancelSubscriptionRenewalAsync([FromBody] CancelSubscriptionRenewalCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("Upgrade")]
        public async Task<IActionResult> RequestSubscriptionUpgradeAsync([FromBody] RequestSubscriptionUpgradeCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpPost("Downgrade")]
        public async Task<IActionResult> RequestSubscriptionDowngradeAsync([FromBody] RequestSubscriptionDowngradeCommand command, CancellationToken cancellationToken = default)
        {
            return EmptyResult(await _mediator.Send(command, cancellationToken));
        }


        [HttpGet("{id}/Cycles")]
        public async Task<IActionResult> GetSubscriptionCyclesListAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ListResult(await _mediator.Send(new GetSubscriptionCyclesQuery(id, null), cancellationToken));
        }


        [HttpGet("{id}/Cycles/{cycleId}")]
        public async Task<IActionResult> GetSubscriptionCycleByCycleIdAsync([FromRoute] Guid id, [FromRoute] Guid cycleId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetSubscriptionCyclesQuery(id, cycleId), cancellationToken);

            return ItemResult(result.Data.FirstOrDefault());
        }


        [HttpGet("{id}/Features")]
        public async Task<IActionResult> GetSubscriptionFeaturesAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return ListResult(await _subscriptionService.GetSubscriptionFeaturesAsync(id, cancellationToken));
        }

        #endregion
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roaa.Rosas.Application;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Controllers;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Framework.Controllers.ApiSimulator
{
    [Route("webhook-simulator")]
    public class WebhookSimulationController : BaseApiController
    {
        #region Props  
        private readonly ISender _mediator;
        private readonly IIdentityContextService _identityContextService;
        private readonly IRosasDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly IInstanceFactory<SubscriptionRenewalHasBeenDisabledBaseEvent, SubscriptionRenewalTypeAttribute> _instanceFactory;

        #endregion

        #region Corts
        public WebhookSimulationController(ISender mediator,
                                           IIdentityContextService identityContextService,
                                           IRosasDbContext dbContext,
                                           IInstanceFactory<SubscriptionRenewalHasBeenDisabledBaseEvent, SubscriptionRenewalTypeAttribute> instanceFactory,
                                           IPublisher publisher)
        {
            _identityContextService = identityContextService;
            _mediator = mediator;
            _publisher = publisher;
            _dbContext = dbContext;
            _instanceFactory = instanceFactory;
        }
        #endregion


        #region Actions    
        [HttpPost("notifyme")]
        public async Task<IActionResult> NotifymeAsync(GlobalPayloadRequest model, CancellationToken cancellationToken = default)
        {
            var info = Request;
            return Ok(model);
        }


        [HttpPost("test")]
        public async Task<IActionResult> TestAsync(CancellationToken cancellationToken = default)
        {

            var sub = await _dbContext.SubscriptionRenewals.FirstOrDefaultAsync(cancellationToken);



            var subscriptionRenewalDisabledEvent = _instanceFactory.CreateInstance(sub.Type.ToString());

            subscriptionRenewalDisabledEvent.SubscriptionRenewal = sub;

            await _publisher.Publish(subscriptionRenewalDisabledEvent);
            return Ok();
        }

        #endregion



        public record GlobalPayloadRequest
        {
            public string TenantSystemName { get; set; } = string.Empty;
            public WebhookEvents EventType { get; set; }
            public dynamic? MetaData { get; set; }
        }
        public record WebhookEndpointRequestModel
        {
            public string Url { get; set; } = string.Empty;
            public string? Secret { get; set; } = string.Empty;
        }
    }
}
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.WebhookEndpoints.EventHandlers
{
    //public class SubscriptionAutoRenewalEnabledEventHandler : IInternalDomainEventHandler<SubscriptionAutorenewalEnabledEvent>
    //{
    //    private readonly IRosasDbContext _dbContext;
    //    private WebhookEvents EventAutoRenewalType = WebhookEvents.AutoRenewalEnabled;

    //    public SubscriptionAutoRenewalEnabledEventHandler(IRosasDbContext dbContext)
    //    {
    //        _dbContext = dbContext;

    //    }

    //    public async Task Handle(SubscriptionAutorenewalEnabledEvent @event, CancellationToken cancelationToken = default)
    //    {
    //        var whEndpoints = await _dbContext.WebhookEndpointEvents.Where(x => x.Event == EventAutoRenewalType)
    //                                              .Select(x => new
    //                                              {
    //                                                  x.WebhookEndpoint!.Url
    //                                              ,
    //                                                  x.WebhookEndpoint.SigningSecret
    //                                              })
    //                                              .ToListAsync(cancelationToken);
    //        whEndpoints.ForEach(whEndpoint =>
    //        {
    //            var CallerModel = new WebhookCallingModel<GlobalPayload<dynamic>>
    //            {
    //                Payload = new GlobalPayload<dynamic>
    //                {
    //                    TenantSystemName = @event.SubscriptionRenewal.Subscription!.Tenant!.SystemName,
    //                    EventType = EventAutoRenewalType,
    //                },
    //                WebhookEndpoints = whEndpoints.Select(x => new WebhookEndpointModel { Secret = x.SigningSecret, Url = x.Url }).ToList(),
    //            };

    //        });
    //        call service of caller


    //    }

    //}


    public class SubscriptionAutoRenewalEnabledEventHandler : BaseWebhookEventHandler<SubscriptionAutorenewalEnabledEvent>
    {
        private readonly IWebhookAPI _webhookAPI;

        public SubscriptionAutoRenewalEnabledEventHandler(IRosasDbContext dbContext, IWebhookAPI webhookAPI) : base(dbContext)
        {
            _webhookAPI = webhookAPI;
        }

        protected override WebhookEvents EventType => WebhookEvents.AutoRenewalEnabled;

        public override async Task Handle(SubscriptionAutorenewalEnabledEvent @event, CancellationToken cancellationToken = default)
        {
            var callerModel = await BuildModel(@event.SubscriptionRenewal.Subscription!.Tenant!.Id, @event, new { autoRenewal = true }, @event.SubscriptionRenewal.Subscription!.Tenant!.SystemName, cancellationToken);
            await _webhookAPI.CallWebhookEndpointsAsync(callerModel, cancellationToken);
        }
    }

}
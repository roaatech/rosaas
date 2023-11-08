using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionRenewedEventHandler : IInternalDomainEventHandler<SubscriptionRenewedEvent>
    {
        private readonly ILogger<SubscriptionRenewedEventHandler> _logger;
        private readonly IPublisher _publisher;

        public SubscriptionRenewedEventHandler(IPublisher publisher,
                                                      ILogger<SubscriptionRenewedEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionRenewedEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionRenewed,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: @event.SystemComment,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}

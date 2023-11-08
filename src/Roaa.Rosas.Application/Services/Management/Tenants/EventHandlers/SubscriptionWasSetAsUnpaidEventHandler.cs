using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionWasSetAsUnpaidEventHandler : IInternalDomainEventHandler<SubscriptionWasSetAsUnpaidEvent>
    {
        private readonly ILogger<SubscriptionWasSetAsUnpaidEventHandler> _logger;
        private readonly IPublisher _publisher;

        public SubscriptionWasSetAsUnpaidEventHandler(IPublisher publisher,
                                                      ILogger<SubscriptionWasSetAsUnpaidEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionWasSetAsUnpaidEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionWasSetAsUnpaidForNonRenewal,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: @event.systemComment,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}

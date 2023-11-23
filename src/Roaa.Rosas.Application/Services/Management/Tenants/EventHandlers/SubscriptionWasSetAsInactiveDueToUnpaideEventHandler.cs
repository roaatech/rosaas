using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionWasSetAsInactiveDueToUnpaideEventHandler : IInternalDomainEventHandler<SubscriptionWasSetAsInactiveDueToUnpaideEvent>
    {
        private readonly ILogger<SubscriptionWasSetAsInactiveDueToUnpaideEventHandler> _logger;
        private readonly IPublisher _publisher;

        public SubscriptionWasSetAsInactiveDueToUnpaideEventHandler(IPublisher publisher,
                                                      ILogger<SubscriptionWasSetAsInactiveDueToUnpaideEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionWasSetAsInactiveDueToUnpaideEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionWasSetAsInactiveDueToNonRenewal,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: @event.systemComment,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}

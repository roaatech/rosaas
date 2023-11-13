using MediatR;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.EventHandlers
{
    public class SubscriptionPlanDowngradedEventHandler : IInternalDomainEventHandler<SubscriptionPlanDowngradedEvent>
    {
        private readonly ILogger<SubscriptionPlanDowngradedEventHandler> _logger;
        private readonly IPublisher _publisher;

        public SubscriptionPlanDowngradedEventHandler(IPublisher publisher,
                                                      ILogger<SubscriptionPlanDowngradedEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(SubscriptionPlanDowngradedEvent @event, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new TenantProcessingCompletedEvent(
                                                   processType: TenantProcessType.SubscriptionDowngraded,
                                                   enabled: true,
                                                   processedData: null,
                                                   comment: string.Empty,
                                                   systemComment: string.Empty,
                                                   processId: out _,
                                                   subscriptions: @event.Subscription));
        }
    }
}
